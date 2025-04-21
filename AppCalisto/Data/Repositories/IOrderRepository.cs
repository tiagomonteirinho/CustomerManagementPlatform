using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<List<Order>> GetAllAsync();

        Task<Order> GetByIdAsync(int id);

        Task<Order> GetByNumberAsync(string number);
    }
}
