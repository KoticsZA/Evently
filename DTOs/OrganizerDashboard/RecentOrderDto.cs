namespace Evently.DTOs.OrganizerDashboard
{
    public class RecentOrderDto
    {
        public int TicketOrderId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
    }
}
