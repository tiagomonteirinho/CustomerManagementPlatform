using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;

namespace CustomerManagementPlatform.Data
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);

        Task<User> GetByIdAsync(string id);

        Task<List<User>> GetAllAsync();

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<List<string>> GetUserRolesAsync(User user);
    }
}
