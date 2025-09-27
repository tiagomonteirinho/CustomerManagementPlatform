using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<List<Order>> GetAllAsync();

        Task<Order> GetByIdAsync(int id);

        Task<List<Order>> GetByTechnicianIdAsync(string technicianId);
    }
}
