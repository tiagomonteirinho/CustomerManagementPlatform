using CustomerManagementPlatform.Helpers;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountHelper _accountHelper;

        public AccountController(IAccountHelper accountHelper)
        {
            _accountHelper = accountHelper;
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
    }
}
