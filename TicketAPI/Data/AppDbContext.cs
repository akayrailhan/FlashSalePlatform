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
            // Havayolu indirim kampanyasındaki çifte satışları (overselling) engellemek için:
            modelBuilder.Entity<Flight>()
                .Property(f => f.Version)
                .IsRowVersion();
        }
    }
}