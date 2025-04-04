using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using AppCalisto.Helpers;
using AppCalisto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICompanyHelper _companyHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository, ICompanyHelper companyHelper, IMailHelper mailHelper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _companyHelper = companyHelper;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();
            }

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserViewModel
            {
                SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>(),
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        { 
            // Load view lists.
            model.SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>();
            model.SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not create user.";
                return View(model);
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ViewBag.Failure = "That email is already being used.";
                return View(model);
            }

            if (model.Roles.Contains("Front-office") && (model.Companies == null || !model.Companies.Any()))
            {
                ViewBag.Failure = "At least one company must be selected for front-office users!";
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
                Companies = model.Roles.Contains("Front-office") ? string.Join(", ", model.Companies) : ""
            };

            if (await _userRepository.CreateAsync(user, null) != IdentityResult.Success)
            {
                ViewBag.Failure = "Could not create user.";
                return View(model);
            }

            await _userRepository.AddToRolesAsync(user, model.Roles);
            foreach (var role in model.Roles)
            {
                if (!await _userRepository.IsInRoleAsync(user, role))
                {
                    ViewBag.Failure = "Could not create user.";
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
                ViewBag.Failure = "Could not send account confirmation email.";
                return View(model);
            }

            ViewBag.Success = "User created successfully!";
            ModelState.Clear(); // Clear view form.
            return View(new UserViewModel
            { 
                // Update view lists.
                SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>(),
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>()
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

            user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();
            return View(user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = (await _userRepository.GetRolesAsync(user)).ToList() ?? new List<string>(),
                SelectableRoles = _roleRepository.GetAll().Where(r => r.Text != "Admin").ToList() ?? new List<SelectListItem>(),
                Companies = string.IsNullOrEmpty(user.Companies) ? new List<string>() : user.Companies.Split(", ").ToList(),
                SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>(),
                LockoutEnd = user.LockoutEnd
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            // Load view lists.
            model.Roles = model.Roles ?? new List<string>();
            model.SelectableRoles = _roleRepository.GetAll().Where(r => r.Text != "Admin").ToList() ?? new List<SelectListItem>();
            model.SelectableCompanies = _companyHelper.GetAll() ?? new List<SelectListItem>();

            if (!ModelState.IsValid)
            {
                ViewBag.Failure = "Could not update user.";
                return View(model);
            }

            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            user.Roles = (await _userRepository.GetRolesAsync(user)).ToList(); // Load user roles.
            if (model.Roles.Contains("Front-office") && (model.Companies == null || !model.Companies.Any()))
            {
                ViewBag.Failure = "At least one company must be selected for front-office users!";
                return View(model);
            }

            if (user.Name == model.Name && user.Email == model.Email && user.Roles.OrderBy(r => r).SequenceEqual(model.Roles.OrderBy(r => r)) && user.Companies == string.Join(", ", model.Companies))
            {
                ViewBag.Failure = "No changes were found.";
                return View(model);
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail != user)
            {
                ViewBag.Failure = "That email is already being used.";
                return View(model);
            }

            if (model.Roles.Contains("Front-office") && (model.Companies == null || !model.Companies.Any()))
            {
                ViewBag.Failure = "At least one company must be selected for front-office users!";
                return View(model);
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Get user roles not included in the model.
            var removedRoles = (user.Roles ?? new List<string>())
                .Except(model.Roles ?? new List<string>())
                .Where(r => r != "Admin")
                .ToList();

            // Get model roles not included in the user.
            var addedRoles = (model.Roles ?? new List<string>())
                .Except(user.Roles ?? new List<string>())
                .Where(r => r != "Admin")
                .ToList();

            await _userRepository.RemoveFromRolesAsync(user, removedRoles);
            await _userRepository.AddToRolesAsync(user, addedRoles);

            user.Companies = model.Roles.Contains("Front-office") ? string.Join(", ", model.Companies) : "";

            if (await _userRepository.UpdateAsync(user) != IdentityResult.Success)
            {
                ViewBag.Failure = "Could not update user.";
                return View(model);
            }

            ViewBag.Success = "User updated successfully.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(string id)
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

            if (await _userRepository.LockOutAsync(user) != IdentityResult.Success)
            {
                ViewBag.Failure = "Could not deactivate user.";
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = (await _userRepository.GetRolesAsync(user)).ToList(),
                SelectableRoles = _roleRepository.GetAll().Where(r => r.Text != "Admin").ToList(),
                LockoutEnd = user.LockoutEnd
            };

            ViewBag.Success = "User deactivated successfully.";
            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Reactivate(string id)
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

            if (await _userRepository.UnlockAsync(user) != IdentityResult.Success)
            {
                ViewBag.Failure = "Could not reactivate user.";
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = (await _userRepository.GetRolesAsync(user)).ToList(),
                SelectableRoles = _roleRepository.GetAll().Where(r => r.Text != "Admin").ToList(),
                LockoutEnd = user.LockoutEnd
            };

            ViewBag.Success = "User reactivated successfully.";
            return View("Edit", model);
        }
    }
}
