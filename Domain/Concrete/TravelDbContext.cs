using System;
using System.Data.Entity;
using Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Concrete
{
    public class TravelDbContext: IdentityDbContext
    {
        public TravelDbContext(): base("TravelDb") 
        {
            Database.SetInitializer(new TravelDInitializer());
        }

        public static TravelDbContext Create()
        {
            return new TravelDbContext();
        }

        public DbSet<Excursion> Excursions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }


    public class TravelDInitializer: DropCreateDatabaseIfModelChanges<TravelDbContext>
    {
        protected override void Seed(TravelDbContext context)
        {
            Excursion excursion = new Excursion()
            { 
                City = "Lviv",
                Date = new DateTime(2017,02,01),
                Description = "Exciting excursion to cultural center of Ukraine",
                Duration = 2,
                PeopleLimit = 6,
                Price = 25,
                Owner = 1
            };
            context.Excursions.Add(excursion);
            AppUserManager userManager = new AppUserManager(new UserStore<User>(context));
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            string guideRoleName = "Guide";
            string travellerRoleName = "Traveller";
            string adminRoleName = "Admin";

            string adminName = "Admin";
            string adminPassword = "MySecret";
            string email = "admin@example.com";

            if (!roleManager.RoleExists(adminRoleName))
            {
                roleManager.Create(new AppRole(adminRoleName));
            }

            if (!roleManager.RoleExists(travellerRoleName))
            {
                roleManager.Create(new AppRole(travellerRoleName));
            }

            if (!roleManager.RoleExists(guideRoleName))
            {
                roleManager.Create(new AppRole(guideRoleName));
            }


            User user = userManager.FindByName(adminName);
            if (user == null)
            {
                userManager.Create(new User { UserName = adminName, Email = email },
                adminPassword);
                user = userManager.FindByName(adminName);
            }

            if (!userManager.IsInRole(user.Id, adminRoleName))
            {
                userManager.AddToRole(user.Id, adminRoleName);
            }

            base.Seed(context);
        }
    }
}
