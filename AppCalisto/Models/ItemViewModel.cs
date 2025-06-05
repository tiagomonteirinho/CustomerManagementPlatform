using AppCalisto.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCalisto.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        public int BudgetId { get; set; }
        public Budget Budget { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 999)]
        public int Quantity { get; set; }

        public IEnumerable<Product> SelectableProducts { get; set; } = new List<Product>();

        [NotMapped]
        public decimal Total => Product != null ? Product.UnitPrice * Quantity : 0m;
    }
}
