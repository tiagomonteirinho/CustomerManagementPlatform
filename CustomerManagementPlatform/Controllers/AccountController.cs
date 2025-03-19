using CustomerManagementPlatform.Data;
using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Helpers;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountHelper _accountHelper;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountController(IAccountHelper accountHelper, IUserRepository userRepository, IConfiguration configuration, IMailHelper mailHelper)
        {
            _accountHelper = accountHelper;
            _userRepository = userRepository;
            _configuration = configuration;
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

            var result = await _accountHelper.LoginAsync(model);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not log in.";
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _accountHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // Prevent view access if authenticated.
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not register user account.";
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
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userRepository.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                ViewBag.ErrorMessage = "Could not register user account.";
                return View(model);
            }

            await _accountHelper.AddToRoleAsync(user, "Customer");
            if (!await _accountHelper.IsInRoleAsync(user, "Customer"))
            {
                ViewBag.ErrorMessage = "Could not register user account.";
            }

            var token = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
            var id = user.Id;
            var tokenUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { token, id },
                protocol: HttpContext.Request.Scheme
            );

            bool emailSent = _mailHelper.SendEmail(user.Email, "Confirm email", $"<h2>Confirm email</h2>"
                + $"To confirm your email and access your account, please click <a href=\"{tokenUrl}\" style=\"color: blue;\">here</a>.");

            if (!emailSent)
            {
                ViewBag.ErrorMessage = "Could not send email confirmation email.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Account created successfully! Instructions to confirm it have been sent to your email address.";
            ModelState.Clear(); // Clear view form.
            return View(new RegisterViewModel()); // Return empty view model.
        }

        public async Task<IActionResult> ChangeDetails()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // Prevent view access if unauthenticated.
            }

            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ChangeDetailsViewModel
            {
                FullName = user.FullName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDetails(ChangeDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not update user details.";
                return View(model);
            }

            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            if (model.FullName == user.FullName)
            {
                ViewBag.SuccessMessage = "No changes detected. No updates were made.";
                return View(model);
            }

            user.FullName = model.FullName;

            var response = await _accountHelper.ChangeDetailsAsync(user);
            if (response.Succeeded)
            {
                ViewBag.SuccessMessage = "User details updated successfully!";
                return View(model);
            }

            ViewBag.ErrorMessage = "Could not update user details.";
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
                }

                var result = await _accountHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Password updated successfully!";
                    return View();
                }

                ViewBag.ErrorMessage = result.Errors.FirstOrDefault().Description;
            }

            return View();
        }

        public IActionResult SendPasswordResetEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendPasswordResetEmail(SendPasswordResetEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Email address not found.";
                    return View(model);
                }

                if (!user.EmailConfirmed)
                {
                    var token = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
                    var id = user.Id;
                    var tokenUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { token, id },
                        protocol: HttpContext.Request.Scheme
                    );

                    bool emailSent = _mailHelper.SendEmail(user.Email, "Confirm email", $"<h2>Confirm email</h2>"
                        + $"To confirm your email and access your account, please click <a href=\"{tokenUrl}\" style=\"color: blue;\">here</a>.");

                    if (!emailSent)
                    {
                        ViewBag.ErrorMessage = "Could not send email confirmation email.";
                        return View(model);
                    }

                    ViewBag.SuccessMessage = "This account has not been confirmed. Instructions to confirm it have been sent to your email address.";
                    return View();
                }
                else
                {
                    var token = await _accountHelper.GeneratePasswordResetTokenAsync(user);
                    var id = user.Id;
                    var tokenUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { token, id },
                        protocol: HttpContext.Request.Scheme
                    );

                    bool emailSent = _mailHelper.SendEmail(user.Email, "Password reset", $"<h2>Password reset</h2>"
                        + $"To reset your password, please update it <a href=\"{tokenUrl}\" style=\"color: blue;\">here</a>.");

                    if (!emailSent)
                    {
                        ViewBag.ErrorMessage = "Could not send password reset email.";
                        return View(model);
                    }

                    ViewBag.SuccessMessage = "Instructions to reset your password have been sent to your email address.";
                    return View();
                }
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token, string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var result = await _accountHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not reset password.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Password updated successfully!";
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _accountHelper.LogoutAsync();
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var result = await _accountHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return RedirectToAction("NotFound404", "Errors");
            }

            return View();
        }
    }
}
