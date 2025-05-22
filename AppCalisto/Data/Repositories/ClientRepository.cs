using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients.AsNoTracking().ToListAsync();
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients.Include(c => c.Orders).ThenInclude(o => o.Service).ThenInclude(s => s.Company).Include(c => c.Orders).ThenInclude(o => o.Technician)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients.Include(c => c.Orders).ThenInclude(o => o.Service).ThenInclude(s => s.Company).Include(c => c.Orders).ThenInclude(o => o.Technician)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> GetByTaxAsync(string tax)
        {
            return await _context.Clients.Include(c => c.Orders).ThenInclude(o => o.Service).ThenInclude(s => s.Company).Include(c => c.Orders).ThenInclude(o => o.Technician)
                .FirstOrDefaultAsync(c => c.Tin == tax);
        }
    }
}
