using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Observation
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }
    }
}
