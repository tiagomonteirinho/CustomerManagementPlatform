using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class ChangeDetailsViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}
