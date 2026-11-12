
using System.ComponentModel.DataAnnotations;

namespace Evently.ViewModels.Bookings
{
    public class TicketBookingViewModel
    {
        public int TicketOrderId { get; set; }

        [Required]
        public string BookingReference { get; set; } = string.Empty;

        public string EventTitle { get; set; } = string.Empty;

        public string? EventCoverImageUrl { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public DateTime EventStartDateTime { get; set; }

        public DateTime EventEndDateTime { get; set; }

        public DateTime BookingDate { get; set; }

        public int TotalTickets { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string BookingStatus { get; set; } = string.Empty;

        public bool IsUpcoming { get; set; }

        public ICollection<BookedTicketViewModel> Tickets { get; set; }
            = new List<BookedTicketViewModel>();
    }

    
}
