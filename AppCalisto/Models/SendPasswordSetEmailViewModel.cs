using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class SendPasswordSetEmailViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
