using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> ExistsAsync(string role);

        Task<IdentityResult> CreateAsync(string role);

        IEnumerable<SelectListItem> GetAll();
    }
}
