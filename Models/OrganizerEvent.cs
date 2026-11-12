namespace Evently.Models
{
    public class OrganizerEvent
    {
        public int OrganizerId { get; set; }
        public EventOrganizer Organizer { get; set; } = null!;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}