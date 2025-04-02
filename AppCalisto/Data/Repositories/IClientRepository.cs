using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IClientRepository : IEntityRepository<Client>
    {
        Task<Client> GetByEmailAsync(string email);

        Task<Client> GetByTaxAsync(string tax);

        IEnumerable<SelectListItem> GetCompanies(int? id);
    }
}
