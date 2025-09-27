using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Data.Entities
{
    public class Service
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Abbreviation { get; set; }

        [Required, MaxLength(99)]
        public string Name { get; set; }

        public Company Company { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }
}
