namespace Evently.DTOs.OrganizerDashboard
{
    public class EventSalesDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public int TicketsSold { get; set; }
        public int Capacity { get; set; }
    }
}
