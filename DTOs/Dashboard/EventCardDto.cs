
using Evently.Models;

namespace Evently.DTOs.Dashboard
{
    public class EventCardDto
    {
        public int EventId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public Category Category { get; set; } 
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal MinimumTicketPrice { get; set; } 
        public bool IsSaved { get; set; }

        public bool IsSoldOut { get; set; }
    }
}
