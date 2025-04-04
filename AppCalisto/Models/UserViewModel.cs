using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "At least one role must be selected.")]
        public List<string> Roles { get; set; }

        public IEnumerable<SelectListItem> SelectableRoles { get; set; }

        [Required(ErrorMessage = "At least one company must be selected.")]
        public List<string> Companies { get; set; } = new List<string>();

        public IEnumerable<SelectListItem> SelectableCompanies { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
