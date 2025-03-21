using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(99)]
        public string Name { get; set; }

        public string Role { get; set; }
    }
}
