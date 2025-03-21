using AppCalisto.Data.Entities;
using AppCalisto.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AppCalisto.Helpers
{
    public interface IAccountHelper
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordSetTokenAsync(User user);

        Task<IdentityResult> SetPasswordAsync(User user, string token, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    }
}
