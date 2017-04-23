using Domain.Entities;
using System;
using System.Collections.Generic;

namespace LetsTravel.Models
{
    public class ExcursionSubscribersViewModel
    {
        public string City { get; set; }
        public string Route { get; set; }
        public DateTime Date { get; set; }
        public double? Duration { get; set; }
        public string Description { get; set; }
        public int? PeopleLimit { get; set; }
        public decimal? Price { get; set; }      
        public  List<User> Subscribers { get; set; }
    }
}