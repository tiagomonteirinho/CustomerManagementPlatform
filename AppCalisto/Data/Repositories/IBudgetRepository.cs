using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IBudgetRepository : IEntityRepository<Budget>
    {
        Task<List<Budget>> GetAllAsync();

        Task<List<Budget>> GetByTechnicianAsync(User technician);

        Task<Budget> GetByIdAsync(int id);
    }
}
