using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class RegisterModel
    {
        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [DisplayName("Traveller")]
        public bool IsTraveller { get; set; }

        [Required]
        [DisplayName("Guide")]
        public bool IsGuide { get; set; }

        [DisplayName("About myself")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }
    }
}