using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required, Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
