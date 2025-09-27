using System;

namespace CustomerManagementPlatform.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public string Title { get; set; }

        public string Message { get; set; }

        public string Action { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
