using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class ClientViewModel
    {
        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [MaxLength(99)]
        public string ContactPerson { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(15)]
        public string Tax { get; set; }
    }
}
