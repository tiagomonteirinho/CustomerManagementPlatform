using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AppCalisto.Data.Entities;
using System.Collections.Generic;

namespace AppCalisto.Data
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);

        Task<User> GetByIdAsync(string id);

        Task<List<User>> GetAllAsync();

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> UpdateAsync(User user);

        Task<IList<string>> GetRolesAsync(User user);

        Task<bool> IsInRoleAsync(User user, string role);

        Task AddToRolesAsync(User user, IEnumerable<string> roles);
    }
}
