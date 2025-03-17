using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CustomerManagementPlatform.Data.Entities;

namespace CustomerManagementPlatform.Data
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
