using CustomerManagementPlatform.Data;
using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Helpers;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountHelper _accountHelper;
        private readonly IUserRepository _userRepository;

        public AccountController(IAccountHelper accountHelper, IUserRepository userRepository)
        {
            _accountHelper = accountHelper;
            _userRepository = userRepository;
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

            var user = await _userRepository.GetUserByEmailAsync(model.Email);
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

            var result = await _userRepository.AddUserAsync(user, model.Password);
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
            var user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
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

            var user = await _userRepository.GetUserByEmailAsync(this.User.Identity.Name);
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
                var user = await _userRepository.GetUserByEmailAsync(this.User.Identity.Name);
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
    }
}
