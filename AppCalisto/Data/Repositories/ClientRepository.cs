using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class ClientRepository : EntityRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> GetByTaxAsync(string tax)
        {
            return await _context.Clients
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Tax == tax);
        }

        public IEnumerable<SelectListItem> GetCompanies(int? id)
        {
            var list = _context.Clients
                .Where(c => c.Id == id)
                .AsEnumerable() // Fetch into memory to allow splitting.
                .SelectMany(c => c.Companies.Split(", "))
                .Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c,
                })
                .OrderBy(i => i.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a company...)",
                Value = string.Empty,
            });

            return list;
        }
    }
}
