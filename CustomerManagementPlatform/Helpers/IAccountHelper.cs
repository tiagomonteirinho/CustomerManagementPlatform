using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Helpers
{
    public interface IAccountHelper
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> ChangeDetailsAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task EnsureCreatedRoleAsync(string role);

        Task<bool> IsInRoleAsync(User user, string role);

        Task AddToRoleAsync(User user, string role);
    }
}
