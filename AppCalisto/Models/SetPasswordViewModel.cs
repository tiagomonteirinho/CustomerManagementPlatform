using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class SetPasswordViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string PasswordSetToken { get; set; }

        public string EmailConfirmationToken { get; set; }
    }
}
