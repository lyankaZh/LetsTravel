using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Entities
{
    public class User: IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AboutMyself { get; set;}

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public virtual List<Excursion> Excursions { get; set; }
        public virtual BlockedUser BlockedUser { get; set; }
    }
}
