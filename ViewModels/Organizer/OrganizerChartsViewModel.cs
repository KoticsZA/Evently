namespace Evently.ViewModels.Organizer
{
    public class OrganizerChartsViewModel
    {
        public List<string> RevenueLabels { get; set; } = new();
        public List<decimal> RevenueValues { get; set; } = new();

        public List<string> EventLabels { get; set; } = new();
        public List<int> EventSold { get; set; } = new();
        public List<int> EventRemaining { get; set; } = new();

        public List<string> TicketTypeLabels { get; set; } = new();
        public List<int> TicketTypeValues { get; set; } = new();
    }
}
