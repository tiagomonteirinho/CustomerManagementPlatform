using System;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Appointment
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public string TechnicianId { get; set; }
        public User Technician { get; set; }

        [Required, MaxLength(109)]
        public string Location { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
