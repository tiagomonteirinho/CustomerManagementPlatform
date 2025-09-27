using CustomerManagementPlatform.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class ObservationViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }

        public List<string> ExistingImageUrls { get; set; } = new();
        public List<IFormFile> ImageFiles { get; set; } = new();
    }
}
