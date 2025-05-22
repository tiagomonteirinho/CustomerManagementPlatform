using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
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
            return await _context.Observations.AsNoTracking().Include(o => o.Order).ToListAsync();
        }

        public async Task<Observation> GetByIdAsync(int id)
        {
            return await _context.Observations.Include(o => o.Order).FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
