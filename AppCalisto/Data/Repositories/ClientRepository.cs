using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
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
    }
}
