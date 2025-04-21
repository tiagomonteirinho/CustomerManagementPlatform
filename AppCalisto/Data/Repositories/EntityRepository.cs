using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private readonly DataContext _context;

        public EntityRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                return await SaveAllAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                return await SaveAllAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
