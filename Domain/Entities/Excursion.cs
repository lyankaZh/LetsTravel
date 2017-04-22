using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Excursion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExcursionId { get; set; }

        public string City { get; set; }
        public string Route { get; set; }
        public DateTime Date { get; set; }
        public double? Duration { get; set; }
        public string Description { get; set; }
        public int? PeopleLimit { get; set; }
        public decimal? Price { get; set; }
        public int? Mark { get; set; }

        public string Guide { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
