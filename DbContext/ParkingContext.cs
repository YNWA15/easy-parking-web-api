using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PublicParkingsSofiaWebAPI.Models;

namespace PublicParkingsSofiaWebAPI
{
    public class ParkingsContext : IdentityDbContext<IdentityUser>
    {

        public ParkingsContext(DbContextOptions<ParkingsContext> options) : base(options) { }
        public virtual DbSet<Parking>? Parkings { get; set; }
        public virtual DbSet<ParkingSpot>? ParkingSpots { get; set; }
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Reservation>? Reservations { get; set; }
        public virtual DbSet<Vehicle>? Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Parking>().HasMany(ps => ps.ParkingSpots).WithOne(p => p.Parking)
                .HasForeignKey(p => p.ParkingId);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Reservations)
                .WithOne(e => e.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ParkingSpot>()
                .HasMany(c => c.Reservations)
                .WithOne(e => e.Spot).HasForeignKey(x => x.SpotId).OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DbCoreConnectionString");
                optionsBuilder.UseSqlServer(connectionString);

            }
        }
    }
}
