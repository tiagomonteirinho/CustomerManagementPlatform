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
            return await _context.Orders.Include(o => o.Client).ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetByNumberAsync(string number)
        {
            return await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.Number == number);
        }
    }
}
