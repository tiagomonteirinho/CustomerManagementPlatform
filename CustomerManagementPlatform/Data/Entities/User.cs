using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementPlatform.Data.Entities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(99)]
        public string Name { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        [NotMapped]
        public IEnumerable<string> Roles { get; set; }
    }
}
