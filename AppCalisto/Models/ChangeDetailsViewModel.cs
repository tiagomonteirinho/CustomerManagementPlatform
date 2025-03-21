using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class ChangeDetailsViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
