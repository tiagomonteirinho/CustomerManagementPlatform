using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Data.Entities
{
    public class Observation
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }

        public List<ObservationImage> Images { get; set; } = new();
    }
}
