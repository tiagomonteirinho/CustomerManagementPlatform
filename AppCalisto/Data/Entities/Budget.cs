using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AppCalisto.Data.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();

        [NotMapped]
        public decimal Total => Items?.Sum(i => i.Total) ?? 0m;
    }
}
