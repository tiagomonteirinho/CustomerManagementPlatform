using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Abbreviation { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        public ICollection<Service> Services { get; set; }
    }
}
