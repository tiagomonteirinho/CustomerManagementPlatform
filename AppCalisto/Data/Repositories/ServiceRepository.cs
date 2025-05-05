using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class ServiceRepository : EntityRepository<Service>, IServiceRepository
    {
        private readonly DataContext _context;

        public ServiceRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services.AsNoTracking().Include(s => s.Company).ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.Include(s => s.Company).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> GetByAbbreviationAsync(string abbreviation)
        {
            return await _context.Services.Include(s => s.Company).FirstOrDefaultAsync(s => s.Abbreviation == abbreviation);
        }
    }
}
