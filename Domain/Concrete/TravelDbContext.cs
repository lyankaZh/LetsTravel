using System.Data.Entity;
using Domain.Entities;

namespace Domain.Concrete
{
    public class TravelDbContext: DbContext
    {
        public TravelDbContext(): base("TravelDb") 
        {
            Database.SetInitializer<TravelDbContext>(new CreateDatabaseIfNotExists<TravelDbContext>());
        }
        public DbSet<Excursion> Excursions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
