namespace Evently.DTOs.OrganizerDashboard
{
    public class OrganizerDashboardDto
    {
        public string OrganizerName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal RevenueLast30Days { get; set; }
        public int TicketsSold { get; set; }
        public int PaidOrders { get; set; }
        public decimal AverageOrderValue => PaidOrders == 0 ? 0m : TotalRevenue / PaidOrders;

        public int TotalEvents { get; set; }
        public int UpcomingEventsCount { get; set; }
        public List<DailyRevenueDto> RevenueTrend { get; set; } = new();
        public List<EventSalesDto> UpcomingEventSales { get; set; } = new();
        public List<TicketTypeSalesDto> TicketMix { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }
}
