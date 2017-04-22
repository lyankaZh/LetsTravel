using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class ExcursionCreationModel
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string Route { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public double Duration { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int PeopleLimit { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}