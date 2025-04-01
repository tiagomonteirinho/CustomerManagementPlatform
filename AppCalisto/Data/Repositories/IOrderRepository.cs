using AppCalisto.Data.Entities;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<Order> GetByNumberAsync(string number);
    }
}
