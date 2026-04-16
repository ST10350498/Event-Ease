using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseManagement.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        // Foreign Keys
        [Required]
        public int VenueId { get; set; }

        [Required]
        public int EventId { get; set; }

        // Navigation properties
        [ForeignKey("VenueId")]
        public virtual Venue? Venue { get; set; }

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }
    }
}