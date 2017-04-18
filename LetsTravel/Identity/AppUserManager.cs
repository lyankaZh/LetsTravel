using LetsTravel.Identity.Models;
using LetsTravel.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace LetsTravel.Identity
{
    public class AppUserManager : UserManager<AppUser>
    {

        public AppUserManager(IUserStore<AppUser> store): base(store)
        {
        }
        public static AppUserManager Create(
        IdentityFactoryOptions<AppUserManager> options,
        IOwinContext context)
        {
            AppIdentityDbContext db = context.Get<AppIdentityDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true
            };

            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                RequireUniqueEmail = true
            };

            return manager;
        }
    }
}
