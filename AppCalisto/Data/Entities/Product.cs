using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Product
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [Required]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        public decimal Tax { get; set; }
    }
}
