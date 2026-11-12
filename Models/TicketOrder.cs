using System.Net.Sockets;

namespace Evently.Models
{
    public class TicketOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N");
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5
    }
}
