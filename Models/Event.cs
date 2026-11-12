using System.ComponentModel;
using System.Net.Sockets;

namespace Evently.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = "South Africa";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;
        public ICollection<OrganizerEvent> Organizers { get; set; } = new List<OrganizerEvent>();
        public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
