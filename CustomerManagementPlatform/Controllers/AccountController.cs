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
            if (ModelState.IsValid)
            {
                var result = await _accountHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Could not login.");
            return View(model);
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
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        UserName = model.Email,
                    };

                    var result = await _userRepository.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Could not register user account.");
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

                    ModelState.AddModelError(string.Empty, "Could not log in.");
                }

                ModelState.AddModelError(string.Empty, "A user already exists with that email address.");
            }

            return View(model);
        }
    }
}
