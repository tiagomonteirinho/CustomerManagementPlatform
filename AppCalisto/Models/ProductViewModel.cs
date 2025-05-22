using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(99)]
        public string Name { get; set; }

        [Required, Range(0, 999999.99), RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The Price field can only have up to 2 decimal places.")]
        public decimal BasePrice { get; set; }

        [Required]
        public int TaxRate { get; set; }

        public List<SelectListItem> SelectableTaxes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "23%", Value = "23" },
            new SelectListItem { Text = "6%", Value = "6" },
            new SelectListItem { Text = "3%", Value = "3" },
            new SelectListItem { Text = "0%", Value = "0" }
        };
    }
}
