using System.ComponentModel.DataAnnotations;

namespace LetsTravel.Models
{
    public class CreateModel
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}