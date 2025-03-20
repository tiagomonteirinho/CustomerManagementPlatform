using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementPlatform.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Full name")]
        [MaxLength(99)]
        public string FullName { get; set; }

        [NotMapped] // Prevent mapping to database.
        public List<string> Roles { get; set; } = new List<string>();
    }
}
