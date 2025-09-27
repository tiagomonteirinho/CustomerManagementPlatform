using CustomerManagementPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public class ObservationRepository : EntityRepository<Observation>, IObservationRepository
    {
        private readonly DataContext _context;

        public ObservationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Observation>> GetAllAsync()
        {
            return await _context.Observations.AsNoTracking()
                .Include(o => o.Order)
                .ToListAsync();
        }

        public async Task<Observation> GetByIdAsync(int id)
        {
            return await _context.Observations
                .Include(o => o.Order)
                .Include(o => o.Images)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public void DeleteImage(ObservationImage image)
        {
            _context.ObservationImages.Remove(image);
        }
    }
}
