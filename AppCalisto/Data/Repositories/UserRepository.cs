using AppCalisto.Data.Entities;
using AppCalisto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public UserRepository(SignInManager<User> signInManager, UserManager<User> userManager, DataContext dataContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = dataContext;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<string> GeneratePasswordSetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> SetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return await _userManager.CreateAsync(user);
            }
            else
            {
                return await _userManager.CreateAsync(user, password);
            }
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> LockOutAsync(User user)
        {
            return await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }

        public async Task<IdentityResult> UnlockAsync(User user)
        {
            return await _userManager.SetLockoutEndDateAsync(user, null);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task AddToRolesAsync(User user, IEnumerable<string> roles)
        {
            await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task RemoveFromRolesAsync(User user, IEnumerable<string> roles)
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IEnumerable<SelectListItem>> GetAllByRoleAsync(string role)
        {
            var users = _context.Users.ToList();
            var roleUsers = new List<SelectListItem>();
            foreach (var user in users) {
                if (await _userManager.IsInRoleAsync(user, role)){
                    roleUsers.Add(new SelectListItem
                    {
                        Text = user.Name,
                        Value = user.Id
                    });
                }
            }

            if (role == "Technician")
            {
                roleUsers.Insert(0, new SelectListItem
                {
                    Text = $"(Select a technician...)",
                    Value = string.Empty
                });
            }
            else
            {
                roleUsers.Insert(0, new SelectListItem
                {
                    Text = $"(Select a user...)",
                    Value = string.Empty
                });
            }

            return roleUsers;
        }
    }
}
