using System.ComponentModel.DataAnnotations;

namespace LetsTravel.Models
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}