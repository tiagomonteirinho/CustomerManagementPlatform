using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IServiceRepository : IEntityRepository<Service>
    {
        Task<List<Service>> GetAllAsync();

        Task<Service> GetByIdAsync(int id);

        Task<Service> GetByAbbreviationAsync(string abbreviation);
    }
}
