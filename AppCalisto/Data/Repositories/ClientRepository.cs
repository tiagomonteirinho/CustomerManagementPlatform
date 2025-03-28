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

        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> GetByTaxAsync(string tax)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Tax == tax);
        }
    }
}
