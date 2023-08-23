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
            // release version
            //optionsBuilder.UseSqlServer("Data Source=31.25.91.22;TrustServerCertificate=True;User ID=sobahnju_hotel;Password=@13931393Pa");
            //optionsBuilder.UseSqlServer("Data Source=.;TrustServerCertificate=True;User ID=sobahnju_hotel;Password=@13931393Pa");

            //demo version on peymanakhtari.com
            //optionsBuilder.UseSqlServer("Data Source=144.76.184.174\\sqlserver2019;TrustServerCertificate=True;User ID=sobahnju_artemisdemo;Password=@13931393Pa");
            optionsBuilder.UseSqlServer("Data Source=.\\sqlserver2019;TrustServerCertificate=True;User ID=sobahnju_artemisdemo;Password=@13931393Pa");




        }
    }
}
