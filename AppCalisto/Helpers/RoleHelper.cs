using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Helpers
{
    public class RoleHelper : IRoleHelper
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleHelper(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task EnsureCreatedRoleAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var list = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
            }).OrderBy(i => i.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a role...)",
                Value = string.Empty,
            });

            return list;
        }
    }
}
