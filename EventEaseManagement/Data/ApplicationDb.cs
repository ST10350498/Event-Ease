using Microsoft.EntityFrameworkCore;
using EventEaseManagement.Models;

namespace EventEaseManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - these represent your database tables
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PREVENT deleting a Venue if it has related Bookings
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Bookings)
                .WithOne(b => b.Venue)
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // PREVENT deleting an Event if it has related Bookings
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Bookings)
                .WithOne(b => b.Event)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // PREVENT double booking - one venue cannot have the same event twice
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.VenueId, b.EventId })
                .IsUnique();
        }
    }
}