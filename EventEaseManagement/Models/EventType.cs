using System.ComponentModel.DataAnnotations;

namespace EventEaseManagement.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EventTypeName { get; set; } = string.Empty;

        // Navigation property - one event type can have many events
        public ICollection<Event>? Events { get; set; }
    }
}