namespace Evently.Models
{
    public class OrganizerEvent
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public EventOrganizer Organizer { get; set; } = null!;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public ICollection<TicketOrder> TicketOrders { get; set; } = new List<TicketOrder>();
    }
}