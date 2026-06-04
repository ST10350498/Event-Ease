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

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; } // NEW for Part 3

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed EventType data (Part 3)
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, EventTypeName = "Conference" },
                new EventType { EventTypeId = 2, EventTypeName = "Wedding" },
                new EventType { EventTypeId = 3, EventTypeName = "Concert" },
                new EventType { EventTypeId = 4, EventTypeName = "Corporate Event" },
                new EventType { EventTypeId = 5, EventTypeName = "Private Party" },
                new EventType { EventTypeId = 6, EventTypeName = "Exhibition" },
                new EventType { EventTypeId = 7, EventTypeName = "Workshop" },
                new EventType { EventTypeId = 8, EventTypeName = "Networking" }
            );

            // Prevent double booking - unique constraint on VenueId + EventId
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.VenueId, b.EventId })
                .IsUnique();
        }
    }
}