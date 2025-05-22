using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCalisto.Data.Entities
{
    public class Item
    {
        public int Id { get; set; }

        public int BudgetId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 999)]
        public int Quantity { get; set; }

        [NotMapped]
        public decimal Total => Product != null ? Product.UnitPrice * Quantity : 0m;
    }
}
