
namespace Evently.DTOs.Dashboard
{
    public class UserDashboardDto
    {
        public string FirstName { get; set; } = string.Empty;

        public int TotalBookings { get; set; }

        public int UpcomingEventsCount { get; set; }

        public int SavedEventsCount { get; set; }

        public TicketBookingDto? NextEvent { get; set; }

        public ICollection<TicketBookingDto> UpcomingEvents { get; set; } = new List<TicketBookingDto>();

        public ICollection<EventCardDto> RecommendedEvents { get; set; }  = new List<EventCardDto>();

        public ICollection<EventCardDto> SavedEvents { get; set; } = new List<EventCardDto>();
    }
}
