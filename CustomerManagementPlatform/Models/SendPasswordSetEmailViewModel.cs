using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class SendPasswordSetEmailViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
