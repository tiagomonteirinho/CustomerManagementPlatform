using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class OrderRepository : EntityRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.AsNoTracking().Include(o => o.Client).Include(o => o.Service).ThenInclude(s => s.Company).Include(o => o.Technician).ToListAsync();
        }

        public async Task<List<Order>> GetByTechnicianAsync(User technician)
        {
            return await _context.Orders.AsNoTracking().Include(o => o.Client).Include(o => o.Service).ThenInclude(s => s.Company).Where(b => b.Technician == technician).ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Client).Include(o => o.Service).ThenInclude(s => s.Company).Include(o => o.Technician).Include(o => o.Budgets).FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
