using AppCalisto.Data;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
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

        public async Task<IActionResult> ChangeDetails()
        {
            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var model = new ChangeDetailsViewModel
            {
                Name = user.Name
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

            if (model.Name == user.Name)
            {
                ViewBag.SuccessMessage = "No changes detected. No updates were made.";
                return View(model);
            }

            user.Name = model.Name;

            var response = await _userRepository.UpdateAsync(user);
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
            if (!ModelState.IsValid)
            {
                return View();
            }

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
            return View();
        }

        public IActionResult SendPasswordResetEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                _accountHelper.LogoutAsync();
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
                var passwordSetToken = await _accountHelper.GeneratePasswordSetTokenAsync(user);
                var actionUrl = Url.Action
                (
                    "SetPassword",
                    "Account",
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
                string passwordSetToken = await _accountHelper.GeneratePasswordSetTokenAsync(user);
                var emailConfirmationToken = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
                var actionUrl = Url.Action
                (
                    "SetPassword",
                    "Account",
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
                _accountHelper.LogoutAsync();
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

            var result = await _accountHelper.SetPasswordAsync(user, model.PasswordSetToken, model.NewPassword);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not set password.";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.EmailConfirmationToken))
            {
                var confirmEmail = await _accountHelper.ConfirmEmailAsync(user, model.EmailConfirmationToken);
                if (!confirmEmail.Succeeded)
                {
                    return RedirectToAction("NotFound404", "Errors");
                }
            }

            ViewBag.SuccessMessage = "Password updated successfully!";
            return View();
        }
    }
}
