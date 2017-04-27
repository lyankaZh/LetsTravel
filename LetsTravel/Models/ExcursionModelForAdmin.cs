using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class ExcursionModelForAdmin
    {
        public int ExcursionId { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public string GuideNickname { get; set; }
        public string GuideFirstName { get; set; }
        public string GuideLastName { get; set; }
    }
}