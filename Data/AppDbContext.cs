using hotelsimorgh.Data.DbModels;
using Microsoft.EntityFrameworkCore;
namespace hotelsimorgh.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<Website> Websites { get; set; }
        //public DbSet<receptionist> receptionists { get; set; }
        public DbSet<passenger> passengers { get; set; }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DeletedReserve> DeletedReserves { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<AlertCheck> AlertChecks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
