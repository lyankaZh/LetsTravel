using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}