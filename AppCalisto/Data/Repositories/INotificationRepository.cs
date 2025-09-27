using AppCalisto.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public interface INotificationRepository : IEntityRepository<Notification>
    {
        //Task<Notification> GetByIdAsync(int id);

        //IQueryable<Notification> GetByUserEmailAsync(string userId);
    }
}
