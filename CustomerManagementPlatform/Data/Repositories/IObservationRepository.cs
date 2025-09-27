using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IObservationRepository : IEntityRepository<Observation>
    {
        Task<List<Observation>> GetAllAsync();

        Task<Observation> GetByIdAsync(int id);

        void DeleteImage(ObservationImage image);
    }
}
