using AppCalisto.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class NotificationRepository : EntityRepository<Notification>, INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<Notification> GetByIdAsync(int id)
        //{
        //    return await _context.Notifications.FindAsync(id);
        //}

        //public IQueryable<Notification> GetByUserEmailAsync(string userId)
        //{
        //    return _context.Notifications
        //            .Include(n => n.User)
        //            .Where(n => n.UserId == userId)
        //            .OrderByDescending(n => n.Time);
        //}
    }
}
