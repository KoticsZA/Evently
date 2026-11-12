
using Evently.Models;
using System.ComponentModel.DataAnnotations;

namespace Evently.ViewModels.Events
{
    public class EventCardViewModel
    {
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? CoverImageUrl { get; set; }

        public Category Category { get; set; } 
        public string VenueName { get; set; }

        public string City { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public decimal MinimumTicketPrice { get; set; }

        public bool IsSaved { get; set; }

        public bool IsSoldOut { get; set; }
    }
}
