using Evently.ViewModels.Bookings;
using Evently.ViewModels.Events;

namespace Evently.ViewModels.Account
{

    public class UserDashboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;

        public int TotalBookings { get; set; }

        public int UpcomingEventsCount { get; set; }

        public int SavedEventsCount { get; set; }

        public TicketBookingViewModel? NextEvent { get; set; }

        public ICollection<TicketBookingViewModel> UpcomingEvents { get; set; } = new List<TicketBookingViewModel>();

        public ICollection<EventCardViewModel> RecommendedEvents { get; set; } = new List<EventCardViewModel>();

        public ICollection<EventCardViewModel> SavedEvents { get; set; } = new List<EventCardViewModel>();
    }

}
