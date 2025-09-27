using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Data.Repositories;
using CustomerManagementPlatform.Helpers;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
                user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserViewModel
            {
                SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not create user.";
                return View(new UserViewModel
                {
                    SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                TempData["Failure"] = "That email is already being used.";
                return View(new UserViewModel
                {
                    SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
            };

            if (await _userRepository.CreateAsync(user, null) != IdentityResult.Success)
            {
                TempData["Failure"] = "Could not create user.";
                return View(new UserViewModel
                {
                    SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            await _userRepository.AddToRolesAsync(user, model.Roles);
            foreach (var role in model.Roles)
            {
                if (!await _userRepository.IsInRoleAsync(user, role))
                {
                    TempData["Failure"] = "Could not create user.";
                    return View(new UserViewModel
                    {
                        SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
                    });
                }
            }

            string passwordSetToken = await _userRepository.GeneratePasswordSetTokenAsync(user);
            string emailConfirmationToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
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
                TempData["Failure"] = "Could not send account confirmation email.";
                return View(new UserViewModel
                {
                    SelectableRoles = _roleRepository.GetAll() ?? new List<SelectListItem>()
                });
            }

            TempData["Success"] = "User created successfully!";
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            user.Roles = (await _userRepository.GetRolesAsync(user)).ToList();
            return View(user);
        }

        private async Task<UserViewModel> BuildEditUserViewModelAsync(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = (await _userRepository.GetRolesAsync(user) ?? new List<string>()).ToList(),
                SelectableRoles = (_roleRepository.GetAll() ?? new List<SelectListItem>()).ToList(),
                LockoutEnd = user.LockoutEnd
            };
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            if (user.Email == "admin@mail")
                return RedirectToAction("Unauthorized401", "Errors");

            user.Roles = (await _userRepository.GetRolesAsync(user) ?? new List<string>()).ToList();
            if (user.Roles.Contains("Admin") && User.Identity.Name != "admin@mail")
                return RedirectToAction("Unauthorized401", "Errors");

            return View(await BuildEditUserViewModelAsync(user));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Could not update user.";
                return View(await BuildEditUserViewModelAsync(user));
            }

            user.Roles = (await _userRepository.GetRolesAsync(user) ?? new List<string>()).ToList();
            if (user.Name == model.Name && user.Email == model.Email && user.Roles.OrderBy(r => r).SequenceEqual(model.Roles.OrderBy(r => r)))
            {
                TempData["Failure"] = "No changes were found.";
                return View(await BuildEditUserViewModelAsync(user));
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail != user)
            {
                TempData["Failure"] = "That email is already being used.";
                return View(await BuildEditUserViewModelAsync(user));
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Get user roles not included in the model.
            var removedRoles = (user.Roles ?? new List<string>())
                .Except(model.Roles ?? new List<string>())
                .ToList();

            await _userRepository.RemoveFromRolesAsync(user, removedRoles);

            // Get model roles not included in the user.
            var addedRoles = (model.Roles ?? new List<string>())
                .Except(user.Roles ?? new List<string>())
                .ToList();

            await _userRepository.AddToRolesAsync(user, addedRoles);

            if (await _userRepository.UpdateAsync(user) != IdentityResult.Success)
            {
                TempData["Failure"] = "Could not update user.";
                return View(await BuildEditUserViewModelAsync(user));
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            if (await _userRepository.LockOutAsync(user) != IdentityResult.Success)
                TempData["Failure"] = "Could not deactivate user.";
            else
                TempData["Success"] = "User deactivated successfully.";

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });

            if (await _userRepository.UnlockAsync(user) != IdentityResult.Success)
                TempData["Failure"] = "Could not reactivate user.";
            else
                TempData["Success"] = "User reactivated successfully.";

            return RedirectToAction("Edit", new { id });
        }
    }
}
