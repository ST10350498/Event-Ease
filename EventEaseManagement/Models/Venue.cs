using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseManagement.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}