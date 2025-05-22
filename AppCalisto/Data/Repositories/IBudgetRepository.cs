using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IBudgetRepository : IEntityRepository<Budget>
    {
        Task<List<Budget>> GetAllAsync();

        Task<Budget> GetByIdAsync(int id);

        Task<Budget> GetByOrderIdAsync(int orderId);
    }
}
