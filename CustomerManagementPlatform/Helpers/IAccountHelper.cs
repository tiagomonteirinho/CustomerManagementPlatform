using CustomerManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Helpers
{
    public interface IAccountHelper
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
