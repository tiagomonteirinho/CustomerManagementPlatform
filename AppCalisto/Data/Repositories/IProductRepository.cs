using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IProductRepository : IEntityRepository<Product>
    {
        Task<List<Product>> GetAllAsync();

        Task<List<Product>> GetByServiceIdAsync(int serviceId);

        Task<Product> GetByIdAsync(int id);

        IEnumerable<SelectListItem> GetAll();
    }
}
