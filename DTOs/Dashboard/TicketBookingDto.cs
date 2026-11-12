
namespace Evently.DTOs.Dashboard
{
    public class TicketBookingDto
    {

        public int TicketOrderId { get; set; }

        public string BookingReference { get; set; } = string.Empty;

        public string EventTitle { get; set; } = string.Empty;
        public int EventId { get; set; }

        public string? EventCoverImageUrl { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public DateTime EventStartDateTime { get; set; }

        public DateTime EventEndDateTime { get; set; }

        public DateTime BookingDate { get; set; }

        public int TotalTickets { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; } = string.Empty;

        public ICollection<BookedTicketDto> Tickets { get; set; } = new List<BookedTicketDto>();
    }
}
