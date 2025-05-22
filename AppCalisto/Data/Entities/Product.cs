using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(99)]
        public string Name { get; set; }

        [Required, Range(0, 999999.99)]
        public decimal BasePrice { get; set; }

        [Required]
        public int TaxRate { get; set; }

        public decimal TaxAmount => BasePrice * TaxRate / 100m;

        public decimal UnitPrice => BasePrice + TaxAmount;
    }
}
