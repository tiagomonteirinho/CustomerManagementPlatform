using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
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
            return await _context.Budgets.AsNoTracking().Include(b => b.Order).ThenInclude(o => o.Technician).ToListAsync();
        }

        public async Task<List<Budget>> GetByTechnicianAsync(User technician)
        {
            return await _context.Budgets.AsNoTracking().Include(b => b.Order).Where(b => b.Order.Technician == technician).ToListAsync();
        }

        public async Task<Budget> GetByIdAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Order).ThenInclude(o => o.Technician)
                .Include(b => b.BudgetProducts).ThenInclude(bp => bp.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
