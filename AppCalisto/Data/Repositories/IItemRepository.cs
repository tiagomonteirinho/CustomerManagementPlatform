using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IItemRepository : IEntityRepository<Item>
    {
        Task<Item> GetByIdAsync(int id);

        Task<List<Item>> GetByBudgetIdAsync(int budgetId);
    }
}
