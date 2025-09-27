using AppCalisto.Data.Repositories;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserRepository userRepository, IMailHelper mailHelper)
        {
            _userRepository = userRepository;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            return View(user);
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Invalid login attempt.";
                return View(model);
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Failure"] = "Could not find that email address.";
                return View(model);
            }

            var result = await _userRepository.LoginAsync(model);
            if (!result.Succeeded)
            {
                TempData["Failure"] = "Could not log in.";
                return View(model);
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            var result = await _userRepository.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password updated successfully!";
                return View();
            }

            TempData["Failure"] = result.Errors.FirstOrDefault().Description;
            return RedirectToAction("ChangePassword");
        }

        public IActionResult SendPasswordResetEmail()
        {
            if (User.Identity.IsAuthenticated)
                _userRepository.LogoutAsync();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendPasswordResetEmail(SendPasswordSetEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Failure"] = "Email address not found.";
                return View(model);
            }

            if (user.EmailConfirmed)
            {
                var passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
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
                    TempData["Failure"] = "Could not send password reset email.";
                    return View(model);
                }

                TempData["Success"] = "Instructions to reset your password have been sent to your email address.";
                return RedirectToAction("SendPasswordResetEmail");
            }
            else
            {
                string passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
                var emailConfirmationToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
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
                    TempData["Failure"] = "Could not send email confirmation email.";
                    return View(model);
                }

                TempData["Success"] = "This account has not been confirmed. Instructions to confirm it and set your password have been sent to your email address.";
                return RedirectToAction("SendPasswordResetEmail");
            }
        }

        public IActionResult SetPassword(string id, string passwordSetToken, string emailConfirmationToken)
        {
            if (User.Identity.IsAuthenticated)
                _userRepository.LogoutAsync();

            return View(new SetPasswordViewModel
            {
                Id = id,
                PasswordSetToken = passwordSetToken,
                EmailConfirmationToken = emailConfirmationToken
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            if (!string.IsNullOrEmpty(model.EmailConfirmationToken))
            {
                var confirmEmail = await _userRepository.ConfirmEmailAsync(user, model.EmailConfirmationToken);
                if (!confirmEmail.Succeeded)
                    return RedirectToAction("NotFound404", "Errors");
            }

            if (await _userRepository.SetPasswordAsync(user, model.PasswordSetToken, model.NewPassword) != IdentityResult.Success)
            {
                TempData["Failure"] = "Could not set password.";
                return View(model);
            }

            TempData["Success"] = "Password updated successfully!";
            return RedirectToAction("SetPassword");
        }
    }
}
