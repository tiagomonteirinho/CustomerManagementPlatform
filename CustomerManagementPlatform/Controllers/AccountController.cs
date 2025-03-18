using CustomerManagementPlatform.Data;
using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Helpers;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

            var loginViewModel = new LoginViewModel
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = false,
            };

            var result2 = await _accountHelper.LoginAsync(loginViewModel);
            if (result2.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Could not register user account.";
            return View(model);
        }

        public async Task<IActionResult> ChangeDetails()
        {
            var user = await _userRepository.GetByEmailAsync(User.Identity.Name);
            var model = new ChangeDetailsViewModel();
            if (user != null)
            {
                model.FullName = user.FullName;
            }

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
        
        [HttpPost]
        public async Task<IActionResult> GenerateApiToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
                }

                var result = await _accountHelper.ValidatePasswordAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken
                    (
                        _configuration["Tokens:Issuer"],
                        _configuration["Tokens:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(15),
                        signingCredentials: credentials
                    );

                    var results = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                    };

                    return Created(string.Empty, results);
                }
            }

            return BadRequest();
        }

        public IActionResult SendPasswordResetEmail()
        {
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

                var token = await _accountHelper.GeneratePasswordResetTokenAsync(user);
                var tokenUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token },
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

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Email address not found.";
                return View(model);
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
    }
}
