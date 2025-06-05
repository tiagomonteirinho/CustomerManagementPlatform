using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public DateTime Creation { get; set; }

        public bool IsUrgent { get; set; }

        [MaxLength(99)]
        public string Status { get; set; }

        public bool IsClosed { get; set; }

        [MaxLength(299)]
        public string ClientDescription { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Required(ErrorMessage = "The Company field is required.")]
        public int CompanyId { get; set; }

        public IEnumerable<SelectListItem> SelectableCompanies { get; set; }

        [Required(ErrorMessage = "The Service field is required.")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public IEnumerable<SelectListItem> SelectableServices { get; set; }

        [Required(ErrorMessage = "The Technician field is required.")]
        public string TechnicianId { get; set; }
        public User Technician { get; set; }
        public IEnumerable<SelectListItem> SelectableTechnicians { get; set; }
    }
}
