using Microsoft.EntityFrameworkCore;
using MetroRailManagementSystem.Models;

namespace MetroRailManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Station> Stations => Set<Station>();
        public DbSet<Train> Trains => Set<Train>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Station>().HasData(
                new Station { Id = 1, Name = "Uttara North", Code = "UN" },
                new Station { Id = 2, Name = "Uttara Center", Code = "UC" },
                new Station { Id = 3, Name = "Agargaon", Code = "AG" }
            );

            modelBuilder.Entity<Train>().HasData(
                new Train { Id = 1, Name = "Metro Express 1", TotalSeats = 200, AvailableSeats = 200, IsActive = true },
                new Train { Id = 2, Name = "Metro Express 2", TotalSeats = 300, AvailableSeats = 300, IsActive = true }
            );
        }

    }
}
