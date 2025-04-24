using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Abbreviation { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }
    }
}
