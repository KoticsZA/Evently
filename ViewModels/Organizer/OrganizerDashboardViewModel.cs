namespace Evently.ViewModels.Organizer
{
    public class OrganizerDashboardViewModel
    {
        public string OrganizerName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal RevenueLast30Days { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TicketsSold { get; set; }
        public int TotalEvents { get; set; }
        public int UpcomingEventsCount { get; set; }

        public bool HasRevenue => TotalRevenue > 0m;
        public bool HasTicketSales => Charts.TicketTypeValues.Count > 0;

        public OrganizerChartsViewModel Charts { get; set; } = new();
        public List<OrganizerOrderRowViewModel> RecentOrders { get; set; } = new();
    }

}
