using AppCalisto.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; }

        public DateTime Execution { get; set; }

        [Required]
        public string Type { get; set; }

        public string Location { get; set; }
        
        public string Description { get; set; }

        public string Status { get; set; }

        public Client Client { get; set; }

        public int? ClientId { get; set; }
    }
}
