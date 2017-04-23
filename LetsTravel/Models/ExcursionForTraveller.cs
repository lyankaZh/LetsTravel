using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace LetsTravel.Models
{
    public class ExcursionForTraveller
    {
        public Excursion Excursion { get; set; }
        public bool CouldBeSubscribed { get; set;}
        public string ReasonForSubscribingDisability { get; set; }
        
    }
}