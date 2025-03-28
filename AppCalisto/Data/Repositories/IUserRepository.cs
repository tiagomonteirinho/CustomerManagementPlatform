using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AppCalisto.Data.Entities;
using System.Collections.Generic;
using AppCalisto.Models;

namespace AppCalisto.Data.Repositories
{
    public interface IUserRepository
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordSetTokenAsync(User user);

        Task<IdentityResult> SetPasswordAsync(User user, string token, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<List<User>> GetAllAsync();

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByIdAsync(string id);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> UpdateAsync(User user);

        Task<IdentityResult> LockOutAsync(User user);

        Task<IdentityResult> UnlockAsync(User user);

        Task<IList<string>> GetRolesAsync(User user);

        Task<bool> IsInRoleAsync(User user, string role);

        Task AddToRolesAsync(User user, IEnumerable<string> roles);

        Task RemoveFromRolesAsync(User user, IEnumerable<string> roles);
    }
}
