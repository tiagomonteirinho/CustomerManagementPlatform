using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMailHelper _mailHelper;

        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository, IMailHelper mailHelper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mailHelper = mailHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
                return View(model);
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Could not find that email address.";
                return View(model);
            }

            var result = await _userRepository.LoginAsync(model);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not log in.";
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();
            }

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserViewModel
            {
                SelectableRoles = _roleRepository.GetAll(),
                Roles = new List<string>()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            model.SelectableRoles = _roleRepository.GetAll(); // Update view roles.
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not create user.";
                return View(model);
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user != null)
            {
                ViewBag.ErrorMessage = "That email is already being used.";
                return View(model);
            }

            user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userRepository.CreateAsync(user, null);
            if (result != IdentityResult.Success)
            {
                ViewBag.ErrorMessage = "Could not create user.";
                return View(model);
            }

            await _userRepository.AddToRolesAsync(user, model.Roles);
            foreach (var role in model.Roles)
            {
                if (!await _userRepository.IsInRoleAsync(user, role))
                {
                    ViewBag.ErrorMessage = "Could not create user.";
                    return View(model);
                }
            }

            string passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
            string emailConfirmationToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
            string actionUrl = Url.Action
            (
                "SetPassword",
                "Users",
                new { id = user.Id, passwordSetToken, emailConfirmationToken },
                protocol: HttpContext.Request.Scheme
            );

            bool emailSent = _mailHelper.SendEmail(user.Email, "Email confirmation", $"<h2>Email confirmation</h2>"
                + $"To confirm your email, please set your password <a href=\"{actionUrl}\" style=\"color: blue;\">here</a>.");
            if (!emailSent)
            {
                ViewBag.ErrorMessage = "Could not send account confirmation email.";
                return View(model);
            }

            ViewBag.SuccessMessage = "User created successfully!";
            ModelState.Clear(); // Clear view form.
            return View(new UserViewModel
            {
                SelectableRoles = _roleRepository.GetAll() // Update view roles.
            });
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var result = await _userRepository.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password updated successfully!";
                return View();
            }

            ViewBag.ErrorMessage = result.Errors.FirstOrDefault().Description;
            return View();
        }

        public IActionResult SendPasswordResetEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                _userRepository.LogoutAsync();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendPasswordResetEmail(SendPasswordSetEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Email address not found.";
                return View(model);
            }

            if (user.EmailConfirmed)
            {
                var passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
                var actionUrl = Url.Action
                (
                    "SetPassword",
                    "Users",
                    new { id = user.Id, passwordSetToken },
                    protocol: HttpContext.Request.Scheme
                );

                bool emailSent = _mailHelper.SendEmail(user.Email, "Password reset", $"<h2>Password reset</h2>"
                    + $"To reset your password, please update it <a href=\"{actionUrl}\" style=\"color: blue;\">here</a>.");
                if (!emailSent)
                {
                    ViewBag.ErrorMessage = "Could not send password reset email.";
                    return View(model);
                }

                ViewBag.SuccessMessage = "Instructions to reset your password have been sent to your email address.";
                return View();
            }
            else
            {
                string passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
                var emailConfirmationToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                var actionUrl = Url.Action
                (
                    "SetPassword",
                    "Users",
                    new { id = user.Id, passwordSetToken, emailConfirmationToken },
                    protocol: HttpContext.Request.Scheme
                );

                bool emailSent = _mailHelper.SendEmail(user.Email, "Email confirmation", $"<h2>Email confirmation</h2>"
                    + $"To confirm your email, please set your password <a href=\"{actionUrl}\" style=\"color: blue;\">here</a>.");
                if (!emailSent)
                {
                    ViewBag.ErrorMessage = "Could not send email confirmation email.";
                    return View(model);
                }

                ViewBag.SuccessMessage = "This account has not been confirmed. Instructions to confirm it and set your password have been sent to your email address.";
                return View();
            }
        }

        public IActionResult SetPassword(string id, string passwordSetToken, string emailConfirmationToken)
        {
            if (User.Identity.IsAuthenticated)
            {
                _userRepository.LogoutAsync();
            }

            return View(new SetPasswordViewModel
            {
                Id = id,
                PasswordSetToken = passwordSetToken,
                EmailConfirmationToken = emailConfirmationToken
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            if (!string.IsNullOrEmpty(model.EmailConfirmationToken))
            {
                var confirmEmail = await _userRepository.ConfirmEmailAsync(user, model.EmailConfirmationToken);
                if (!confirmEmail.Succeeded)
                {
                    return RedirectToAction("NotFound404", "Errors");
                }
            }

            var result = await _userRepository.SetPasswordAsync(user, model.PasswordSetToken, model.NewPassword);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not set password.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Password updated successfully!";
            return View();
        }
    }
}
