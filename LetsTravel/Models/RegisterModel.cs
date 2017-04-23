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
        [Required(ErrorMessage = "Please enter a nickname")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Please enter an email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
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