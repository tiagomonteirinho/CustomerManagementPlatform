using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Execution { get; set; } = DateTime.Now;

        public DateTime Appointment { get; set; } = DateTime.Now;

        public bool IsUrgent { get; set; }

        public string Location { get; set; }

        [MaxLength(299)]
        public string Description { get; set; }

        public Client Client { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Service Service { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public User Technician { get; set; }

        [Required]
        public string TechnicianId { get; set; }

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
