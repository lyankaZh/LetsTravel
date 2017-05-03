using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class UserForAdminViewModel
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required (ErrorMessage = "Enter reason for blocking")]
        public string Reason { get; set; }
        public string CollapseId { get; set; }
        public bool IsBlocked { get; set; }
    }
}