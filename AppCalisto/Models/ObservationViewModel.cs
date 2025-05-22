using AppCalisto.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class ObservationViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }
    }
}
