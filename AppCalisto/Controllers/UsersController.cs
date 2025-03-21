using AppCalisto.Data;
using AppCalisto.Data.Entities;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountHelper _accountHelper;
        private readonly IRoleHelper _roleHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(IUserRepository userRepository, IRoleHelper roleHelper, IAccountHelper accountHelper, IMailHelper mailHelper)
        {
            _userRepository = userRepository;
            _roleHelper = roleHelper;
            _accountHelper = accountHelper;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserViewModel
            {
                Roles = _roleHelper.GetAll()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            model.Roles = _roleHelper.GetAll(); // Update view roles.
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

            await _userRepository.AddToRoleAsync(user, model.Role);
            var isInRole = await _userRepository.IsInRoleAsync(user, model.Role);
            if (!isInRole)
            {
                ViewBag.ErrorMessage = "Could not create user.";
                return View(model);
            }

            string passwordSetToken = await _accountHelper.GeneratePasswordSetTokenAsync(user);
            string emailConfirmationToken = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
            string actionUrl = Url.Action
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
                ViewBag.ErrorMessage = "Could not send account confirmation email.";
                return View(model);
            }

            ViewBag.SuccessMessage = "User created successfully!";
            ModelState.Clear(); // Clear view form.
            return View(new UserViewModel
            {
                Roles = _roleHelper.GetAll() // Update view roles.
            });
        }

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

            return View(user);
        }
    }
}
