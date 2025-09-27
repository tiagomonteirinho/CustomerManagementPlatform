using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(5)]
        public string Abbreviation { get; set; }

        [Required, MaxLength(99)]
        public string Name { get; set; }
    }
}
