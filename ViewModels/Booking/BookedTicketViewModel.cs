namespace Evently.ViewModels.Bookings
{
    public class BookedTicketViewModel
    {
        public string TicketNumber { get; set; } = string.Empty;

        public string TicketTypeName { get; set; } = string.Empty;

        public string QRCodeValue { get; set; } = string.Empty;

        public string TicketStatus { get; set; } = string.Empty;
    }
}
