using CustomerManagementPlatform.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerManagementPlatform.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(99)]
        public string Name { get; set; }

        [MaxLength(99)]
        public string ContactPerson { get; set; }

        [Required, MaxLength(99)]
        public string Address { get; set; }

        [Required, MaxLength(8)]
        public string ZipCode { get; set; }

        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(15)]
        public string Tin { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
