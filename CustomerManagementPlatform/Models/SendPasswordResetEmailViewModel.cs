using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class SendPasswordResetEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
