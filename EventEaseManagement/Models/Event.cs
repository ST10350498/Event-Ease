using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseManagement.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        // NEW: Foreign key for EventType (Part 3)
        [ForeignKey("EventType")]
        public int? EventTypeId { get; set; }

        // NEW: Navigation property for EventType (Part 3)
        public virtual EventType? EventType { get; set; }

        // Navigation property - an event can have many bookings
        public ICollection<Booking>? Bookings { get; set; }
    }
}