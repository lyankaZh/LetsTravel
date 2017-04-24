using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace LetsTravel.Models
{
    public class ExcursionWithGuideInfoViewModel
    {
        public int ExcursionId { get; set; }
        public string City { get; set; }
        public string Route { get; set; }
        public DateTime Date { get; set; }
        public double? Duration { get; set; }
        public string Description { get; set; }
        public int? PeopleLimit { get; set; }
        public decimal? Price { get; set; }
        public User Guide { get; set; }
        public string ModalId { get; set; }
    }
}