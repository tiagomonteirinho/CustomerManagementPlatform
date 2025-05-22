using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AppCalisto.Models
{
    public class BudgetViewModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required(ErrorMessage = "At least one product must be selected.")]
        public List<ItemViewModel> Items { get; set; } = new();

        [NotMapped]
        public decimal Total => Items?.Sum(i => i.Total) ?? 0m;
    }
}
