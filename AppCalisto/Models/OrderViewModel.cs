using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace AppCalisto.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public DateTime Creation { get; set; }

        public DateTime Execution { get; set; }

        public DateTime Appointment { get; set; }

        public bool IsUrgent { get; set; }

        public string Location { get; set; }
        
        public string Description { get; set; }

        public string Status { get; set; }

        public Client Client { get; set; }

        public int ClientId { get; set; }

        public int CompanyId { get; set; }

        public IEnumerable<SelectListItem> SelectableCompanies { get; set; }

        public Service Service { get; set; }

        public int ServiceId { get; set; }

        public IEnumerable<SelectListItem> SelectableServices { get; set; }
    }
}
