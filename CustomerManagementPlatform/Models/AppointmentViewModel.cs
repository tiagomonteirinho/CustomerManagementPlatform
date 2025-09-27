using CustomerManagementPlatform.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System;

namespace CustomerManagementPlatform.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string TechnicianId { get; set; }
        public User Technician { get; set; }

        [Required, MaxLength(109)]
        public string Location { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);
    }
}
