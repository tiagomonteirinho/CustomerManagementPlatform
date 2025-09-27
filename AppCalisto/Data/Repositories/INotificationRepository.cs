using CustomerManagementPlatform.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data.Repositories
{
    public interface INotificationRepository : IEntityRepository<Notification>
    {
        //Task<Notification> GetByIdAsync(int id);

        //IQueryable<Notification> GetByUserEmailAsync(string userId);
    }
}
