using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Client).Include(o => o.Service).ThenInclude(s => s.Company).Include(o => o.Technician).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetByNumberAsync(string number)
        {
            return await _context.Orders.Include(o => o.Client).Include(o => o.Service).ThenInclude(s => s.Company).Include(o => o.Technician).FirstOrDefaultAsync(o => o.Number == number);
        }
    }
}
