using System.Net.Sockets;

namespace Evently.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantitySold { get; set; } = 0;
        public DateTime? SalesStartDate { get; set; }
        public DateTime? SalesEndDate { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}