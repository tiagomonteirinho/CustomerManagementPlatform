using AppCalisto.Data.Entities;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IClientRepository : IEntityRepository<Client>
    {
        Task<Client> GetByEmailAsync(string email);

        Task<Client> GetByTaxAsync(string tax);
    }
}
