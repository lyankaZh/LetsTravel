using Domain.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Domain.Entities
{
    public class AppUserManager : UserManager<User>
    {

        public AppUserManager(IUserStore<User> store): base(store)
        {
        }
        public static AppUserManager Create(
        IdentityFactoryOptions<AppUserManager> options,
        IOwinContext context)
        {
            TravelDbContext db = context.Get<TravelDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<User>(db));
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true
            };

            manager.UserValidator = new UserValidator<User>(manager)
            {
                RequireUniqueEmail = true
            };

            return manager;
        }
    }
}
