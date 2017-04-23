using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class ExcursionCreationModel
    {
        [Required(ErrorMessage = "Please enter a city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a route")]
        public string Route { get; set; }

        [Required(ErrorMessage = "Please enter a date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter a duration")]
        public double Duration { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a limit of people")]
        public int PeopleLimit { get; set; }

        [Required(ErrorMessage = "Please enter a price")]
        public decimal Price { get; set; }
    }
}