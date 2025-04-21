using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface ICompanyRepository : IEntityRepository<Company>
    {
        Task<List<Company>> GetAllAsync();

        Task<Company> GetByIdAsync(int id);

        Task<Company> GetByAbbreviationAsync(string abbreviation);

        IEnumerable<SelectListItem> GetAll();

        IEnumerable<SelectListItem> GetServices(int companyId);
    }
}
