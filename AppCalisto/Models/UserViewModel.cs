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

        [Required(ErrorMessage = "At least one role must be selected")]
        public List<string> Roles { get; set; } = new List<string>();

        public IEnumerable<SelectListItem> SelectableRoles { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
