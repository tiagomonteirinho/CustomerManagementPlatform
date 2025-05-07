using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<List<Order>> GetAllAsync();

        Task<List<Order>> GetByTechnicianAsync(User technician);

        Task<Order> GetByIdAsync(int id);
    }
}
