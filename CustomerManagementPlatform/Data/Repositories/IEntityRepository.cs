using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface IEntityRepository<T> where T : class
    {
        Task<bool> SaveAllAsync();

        Task CreateAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);
    }
}
