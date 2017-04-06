using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Excursion
    {
        public int ExcursionId { get; set; }
        public string City { get; set; }
        public string Route { get; set; }
        public DateTime Date { get; set; }
        public double? Duration { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public List<User> Subscribers { get; set; }
        public int? PeopleLimit { get; set; }
        public decimal Price { get; set; }
        public Mark Rate { get; set; }

        public virtual int RateId
        {
            get { return (int)Rate; }
            set { Rate = (Mark)value; }
        }
    }
}
