using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IClientRepository : IEntityRepository<Client>
    {
        Task<List<Client>> GetAllAsync();

        Task<Client> GetByIdAsync(int id);

        Task<Client> GetByEmailAsync(string email);

        Task<Client> GetByTaxAsync(string tax);
    }
}
