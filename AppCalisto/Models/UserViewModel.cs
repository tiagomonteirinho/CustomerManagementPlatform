using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class UserViewModel
    {
        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [Required(ErrorMessage = "A role must be selected")]
        public string Role { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
