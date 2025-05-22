using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface IObservationRepository : IEntityRepository<Observation>
    {
        Task<List<Observation>> GetAllAsync();

        Task<Observation> GetByIdAsync(int id);
    }
}
