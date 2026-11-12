using Evently.Data;
using Evently.DTOs;
using Evently.DTOs.OrganizerDashboard;
using Evently.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evently.Services
{
    public class OrganizerDashboardService : IOrganizerDashboardService
    {
        private const string PaidStatus = "Paid";
        private const int TrendDays = 30;

        private readonly EventlyDBContext _context;      

        public OrganizerDashboardService(EventlyDBContext context)
        {
            _context = context;
        }

        public async Task<OrganizerDashboardDto> GetDashboardAsync(string userId, CancellationToken cancellationToken)
        {
            var organizer = await _context.EventOrganizers
                .AsNoTracking()
                .Where(o => o.OwnerUserId == userId)
                .Select(o => new { o.Id, o.Name, o.IsVerified })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new KeyNotFoundException($"No organizer profile exists for user '{userId}'.");

            var orgId = organizer.Id;
            var now = DateTime.UtcNow;
            var since = now.Date.AddDays(-(TrendDays - 1));

            var orders = _context.TicketOrders.AsNoTracking()
                .Where(o => o.OrganizerEvent.OrganizerId == orgId);

            var paidOrders = orders.Where(o => o.Status == Models.OrderStatus.Paid);

            var events = _context.OrganizerEvents.AsNoTracking()
                .Where(e => e.OrganizerId == orgId);

            var totals = await paidOrders
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Revenue = g.Sum(o => o.TotalAmount),
                    Tickets = g.Sum(o => o.Tickets.Count),
                    Orders = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            var daily = await paidOrders
                .Where(o => o.CreatedAt >= since)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(o => o.TotalAmount) })
                .ToListAsync(cancellationToken);

            var byDay = daily.ToDictionary(d => d.Date, d => d.Amount);
            var trend = Enumerable.Range(0, TrendDays)
                .Select(i => since.AddDays(i))
                .Select(day => new DailyRevenueDto { Date = day, Amount = byDay.GetValueOrDefault(day) })
                .ToList();

            var totalEvents = await events.CountAsync(cancellationToken);
            var upcomingCount = await events.CountAsync(e => e.Event.StartDate >= now, cancellationToken);

            var upcomingSales = await events
                .Where(e => e.Event.StartDate >= now)
                .OrderBy(e => e.Event.StartDate)
                .Take(6)
                .Select(e => new EventSalesDto
                {
                    EventId = e.EventId,
                    Title = e.Event.Title,
                    StartDateTime = e.Event.StartDate,
                    TicketsSold = e.TicketOrders
                        .Where(o => o.Status == Models.OrderStatus.Paid)
                        .Sum(o => (int?)o.Tickets.Count) ?? 0,
                    Capacity = e.Event.TicketTypes.Sum(t => (int?)t.QuantityAvailable) ?? 0
                })
                .ToListAsync(cancellationToken);

            var mix = await _context.Tickets.AsNoTracking()
                .Where(t => t.TicketOrder.OrganizerEvent.OrganizerId == orgId
                         && t.TicketOrder.Status == Models.OrderStatus.Paid)
                .GroupBy(t => t.TicketType.Name)
                .Select(g => new TicketTypeSalesDto { Name = g.Key, Sold = g.Count() })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync(cancellationToken);

            var recent = await orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(6)
                .Select(o => new RecentOrderDto
                {
                    TicketOrderId = o.Id,
                    BookingReference = o.OrderNumber,
                    EventTitle = o.OrganizerEvent.Event.Title,
                    TotalTickets = o.Tickets.Count,
                    TotalAmount = o.TotalAmount,
                    PaymentStatus = o.Status.ToString(),
                    BookingDate = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new OrganizerDashboardDto
            {
                OrganizerName = organizer.Name,
                IsVerified = organizer.IsVerified,

                TotalRevenue = totals?.Revenue ?? 0m,
                TicketsSold = totals?.Tickets ?? 0,
                PaidOrders = totals?.Orders ?? 0,
                RevenueLast30Days = trend.Sum(t => t.Amount),

                TotalEvents = totalEvents,
                UpcomingEventsCount = upcomingCount,

                RevenueTrend = trend,
                UpcomingEventSales = upcomingSales,
                TicketMix = mix,
                RecentOrders = recent
            };
        }
    }
}
