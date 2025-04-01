using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppCalisto.Data.Entities
{
    public class Client : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [MaxLength(99)]
        public string ContactPerson { get; set; }

        public string Email { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(15)]
        public string Tax { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
