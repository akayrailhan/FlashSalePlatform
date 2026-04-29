using Microsoft.EntityFrameworkCore;
using TicketAPI.Models;

namespace TicketAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // to prevent overselling, we use a concurrency token (row version)
            modelBuilder.Entity<Flight>()
                .Property(f => f.Version)
                .IsRowVersion();
        }
    }
}