using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class ItemRepository : EntityRepository<Item>, IItemRepository
    {
        private readonly DataContext _context;

        public ItemRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _context.Items
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Item>> GetByBudgetIdAsync(int budgetId)
        {
            return await _context.Items
                .Include(t => t.Product)
                .Where(t => t.BudgetId == budgetId)
                .ToListAsync();
        }
    }
}
