using CustomerManagementPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
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
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Service).ThenInclude(s => s.Company)
                .Include(o => o.Technician)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Service).ThenInclude(s => s.Company)
                .Include(o => o.Technician)
                .Include(o => o.Observation).ThenInclude(o => o.Images)
                .Include(o => o.Budget).ThenInclude(b => b.Items).ThenInclude(i => i.Product)
                .Include(o => o.Appointments)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByTechnicianIdAsync(string technicianId)
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Service).ThenInclude(s => s.Company)
                .Include(o => o.Technician)
                .Include(o => o.Observation).ThenInclude(o => o.Images)
                .Include(o => o.Budget).ThenInclude(b => b.Items).ThenInclude(i => i.Product)
                .Include(o => o.Appointments)
                .Where(o => o.TechnicianId == technicianId).ToListAsync();
        }
    }
}
