using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class BudgetViewModel
    {
        public int Id { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }

        public Order Order { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "At least one product must be selected.")]
        public List<int> ProductIds { get; set; }

        public List<BudgetProductViewModel> Products { get; set; }

        public IEnumerable<SelectListItem> SelectableProducts { get; set; }

        public string Status { get; set; }
    }
}
