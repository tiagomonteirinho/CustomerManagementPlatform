using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CustomerManagementPlatform.Data.Entities;

namespace CustomerManagementPlatform.Data
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);

        Task<User> GetByIdAsync(string id);

        Task<IdentityResult> CreateAsync(User user, string password);
    }
}
