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

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    }
}
