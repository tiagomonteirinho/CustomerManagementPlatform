using CustomerManagementPlatform.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public class CompanyRepository : EntityRepository<Company>, ICompanyRepository
    {
        private readonly DataContext _context;

        public CompanyRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.AsNoTracking().ToListAsync();
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            return await _context.Companies.Include(c => c.Services).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Company> GetByAbbreviationAsync(string abbreviation)
        {
            return await _context.Companies.Include(c => c.Services).FirstOrDefaultAsync(c => c.Abbreviation == abbreviation);
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var companies = _context.Companies.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            })
            .ToList();

            companies.Insert(0, new SelectListItem
            {
                Text = "(Select a company...)",
                Value = string.Empty
            });

            return companies;
        }

        public IEnumerable<SelectListItem> GetServices(int companyId)
        {
            var services = _context.Services.Where(s => s.CompanyId == companyId)
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                })
                .ToList();

            services.Insert(0, new SelectListItem
            {
                Text = "(Select a service...)",
                Value = string.Empty
            });

            return services;
        }
    }
}
