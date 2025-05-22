using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public bool IsUrgent { get; set; }

        public string Status { get; set; }

        [MaxLength(299)]
        public string ClientDescription { get; set; }

        public Client Client { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Service Service { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public User Technician { get; set; }

        [Required]
        public string TechnicianId { get; set; }

        public Observation Observation { get; set; }

        public Budget Budget { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
