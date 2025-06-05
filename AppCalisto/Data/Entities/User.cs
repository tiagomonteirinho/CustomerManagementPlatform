using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCalisto.Data.Entities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(99)]
        public string Name { get; set; }

        [NotMapped]
        public IEnumerable<string> Roles { get; set; }
    }
}
