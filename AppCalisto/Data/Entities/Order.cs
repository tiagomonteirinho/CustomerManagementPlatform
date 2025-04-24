using System;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Number { get; set; } = Guid.NewGuid().ToString();

        public DateTime Creation { get; set; } = DateTime.Now;

        public DateTime Execution { get; set; } = DateTime.Now;

        public DateTime Appointment { get; set; } = DateTime.Now;

        public bool IsUrgent { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public Client Client { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Service Service { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public User Technician { get; set; }

        [Required]
        public string TechnicianId { get; set; }
    }
}
