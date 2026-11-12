
namespace Evently.DTOs.Dashboard
{
    public class BookedTicketDto
    {
        public string TicketNumber { get; set; } = string.Empty;

        public string TicketTypeName { get; set; } = string.Empty;

        public string QRCodeValue { get; set; } = string.Empty;

        public string TicketStatus { get; set; } = string.Empty;
    }
}
