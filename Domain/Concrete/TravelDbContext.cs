using System;
using System.Data.Entity;
using Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Concrete
{
    public class TravelDbContext: IdentityDbContext<User>
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
        //public new DbSet<User> Users { get; set; }

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
            
            AppUserManager userManager = new AppUserManager(new UserStore<User>(context));
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            string guideRoleName = "Guide";
            string travellerRoleName = "Traveller";
            string adminRoleName = "Admin";

            string adminNick = "Admin";
            string adminFirstName = "Admin";
            string adminLastName = "Admin";
            string adminPassword = "MySecret";
            string adminEmail = "admin@example.com";

            string guideNick = "guide";
            string guideFirstName = "Guide";
            string guideLastName = "Guide";
            string guidePassword = "123qqq";
            string guideEmail = "guide@example.com";

            string travellerNick = "traveller";
            string travellerFirstName = "Traveller";
            string travellerLastName = "Traveller";
            string travellerPassword = "123qqq";
            string travellerEmail = "traveller@example.com";


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


            CreateTemplateUser(adminNick, adminFirstName, adminLastName, adminEmail, adminPassword, userManager,
                adminRoleName);
            CreateTemplateUser(travellerNick, travellerFirstName, travellerLastName, travellerEmail, travellerPassword, userManager,
                travellerRoleName);
            var guide = CreateTemplateUser(guideNick, guideFirstName, guideLastName, guideEmail, guidePassword, userManager,
                guideRoleName);

            Excursion excursion = new Excursion()
            {
                City = "Lviv",
                Date = new DateTime(2017, 02, 01),
                Description = "Exciting excursion to cultural center of Ukraine",
                Duration = 2,
                PeopleLimit = 6,
                Price = 25,
                Route = "Old castles",
                Guide = guide.Id
            };
            context.Excursions.Add(excursion);
            base.Seed(context);
        }

        private User CreateTemplateUser(string nick, 
                                        string first, string last, string email, string pass, 
                                        AppUserManager userManager, string role)
        {
            User user = userManager.FindByName(nick);
            if (user == null)
            {
                userManager.Create(
                    new User { UserName = nick, FirstName = first, LastName = last, Email = email }, pass);
                user = userManager.FindByName(nick);
            }

            if (!userManager.IsInRole(user.Id, role))
            {
                userManager.AddToRole(user.Id, role);
            }
            return user;
        }
    }
}
