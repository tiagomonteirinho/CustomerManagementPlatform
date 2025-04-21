using AppCalisto.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class ServiceViewModel
    {
        public int Id { get; set; }

        public string Abbreviation { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        public Company Company { get; set; }

        public int CompanyId { get; set; }
    }
}
