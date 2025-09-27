using CustomerManagementPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public class BudgetRepository : EntityRepository<Budget>, IBudgetRepository
    {
        private readonly DataContext _context;

        public BudgetRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllAsync()
        {
            return await _context.Budgets.AsNoTracking()
                .Include(b => b.Order).ThenInclude(o => o.Technician)
                .ToListAsync();
        }

        public async Task<Budget> GetByIdAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Order).ThenInclude(o => o.Technician)
                .Include(b => b.Order).ThenInclude(o => o.Service).ThenInclude(s => s.Company)
                .Include(b => b.Items).ThenInclude(bp => bp.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Budget> GetByOrderIdAsync(int orderId)
        {
            return await _context.Budgets
                .Include(b => b.Order).ThenInclude(o => o.Technician)
                .Include(b => b.Order).ThenInclude(o => o.Service).ThenInclude(s => s.Company)
                .Include(b => b.Items).ThenInclude(bp => bp.Product)
                .FirstOrDefaultAsync(b => b.Order.Id == orderId);
        }
    }
}
