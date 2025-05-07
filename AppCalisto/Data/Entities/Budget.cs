using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCalisto.Data.Entities
{
    public class Budget
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }

        public Order Order { get; set; }

        [Required]
        public int OrderId { get; set; }

        public ICollection<BudgetProduct> BudgetProducts { get; set; } = new List<BudgetProduct>();

        [NotMapped]
        public List<int> ProductIds { get; set; } = new();

        public decimal Total { get; set; }

        public string Status { get; set; }
    }
}
