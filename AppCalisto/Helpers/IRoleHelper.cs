using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Helpers
{
    public interface IRoleHelper
    {
        Task EnsureCreatedRoleAsync(string role);

        IEnumerable<SelectListItem> GetAll();
    }
}
