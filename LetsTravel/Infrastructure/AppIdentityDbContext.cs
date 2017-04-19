using System.Data.Entity;
using Domain.Entities;
using LetsTravel.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LetsTravel.Infrastructure
{
    //public class AppIdentityDbContext : IdentityDbContext<User>
    //{
    //    public AppIdentityDbContext() : base("TravelDb") { }

    //    static AppIdentityDbContext()
    //    {
    //        Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
    //    }
    //    public static AppIdentityDbContext Create()
    //    {
    //        return new AppIdentityDbContext();
    //    }
    //}

    //public class IdentityDbInit
    //: DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    //{
    //    protected override void Seed(AppIdentityDbContext context)
    //    {
    //        PerformInitialSetup(context);
    //        base.Seed(context);
    //    }

    //    public void PerformInitialSetup(AppIdentityDbContext context)
    //    {
    //        AppUserManager userManager = new AppUserManager(new UserStore<User>(context));
    //        AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

    //        string guideRoleName = "Guide";
    //        string travellerRoleName = "Traveller";
    //        string adminRoleName = "Administrator";

    //        string adminName = "Admin";
    //        string adminPassword = "MySecret";
    //        string email = "admin@example.com";

    //        if (!roleManager.RoleExists(adminRoleName))
    //        {
    //            roleManager.Create(new AppRole(adminRoleName));
    //        }

    //        if (!roleManager.RoleExists(travellerRoleName))
    //        {
    //            roleManager.Create(new AppRole(travellerRoleName));
    //        }

    //        if (!roleManager.RoleExists(guideRoleName))
    //        {
    //            roleManager.Create(new AppRole(guideRoleName));
    //        }


    //        User user = userManager.FindByName(adminName);
    //        if (user == null)
    //        {
    //            userManager.Create(new User { UserName = adminName, Email = email },
    //            adminPassword);
    //            user = userManager.FindByName(adminName);
    //        }

    //        if (!userManager.IsInRole(user.Id, adminName))
    //        {
    //            userManager.AddToRole(user.Id, adminName);
    //        }
    //    }
    //}
}