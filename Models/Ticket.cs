namespace Evently.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = Guid.NewGuid().ToString("N");
        public string QRCodeValue { get; set; }  = Guid.NewGuid().ToString("N");
        public TicketStatus Status { get; set; }  = TicketStatus.Valid;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UsedAt { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketOrderId { get; set; }
        public TicketType TicketType { get; set; } = null!;
        public TicketOrder TicketOrder { get; set; } = null!;
    }
    public enum TicketStatus
    {
        Valid = 1,
        Used = 2,
        Cancelled = 3,
        Refunded = 4
    }
}