using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
