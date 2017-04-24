using System.ComponentModel.DataAnnotations;

namespace LetsTravel.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter a nickname")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
    }
}