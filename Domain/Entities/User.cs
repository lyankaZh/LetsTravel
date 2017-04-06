using System.Collections.Generic;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public virtual List<Excursion> Excursions { get; set; }
    }
}
