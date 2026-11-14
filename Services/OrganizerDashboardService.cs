using Evently.Data;
using Evently.DTOs;
using Evently.DTOs.OrganizerDashboard;
using Evently.Interfaces;
using Evently.Models;
using Microsoft.EntityFrameworkCore;

namespace Evently.Services
{
    public class OrganizerDashboardService : IOrganizerDashboardService
    {
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

            // The organizer's events through the OrganizerEvent join
            var events = _context.OrganizerEvents.AsNoTracking()
                .Where(oe => oe.OrganizerId == orgId)
                .Select(oe => oe.Event);

            // Keep orders as IQueryable to allow async database aggregations
            var orders = _context.TicketOrders.AsNoTracking()
                .Where(o => _context.OrganizerEvents.Any(oe => oe.OrganizerId == orgId && oe.Id == o.OrganizerEventId));

            var paidOrders = orders.Where(o => o.Status == OrderStatus.Paid);

            // 1. Totals (Executed in SQL)
            var totals = await paidOrders
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Revenue = g.Sum(o => o.TotalAmount),
                    Tickets = g.Sum(o => o.Tickets.Count),
                    Orders = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            // 2. Daily revenue, last 30 days (Executed in SQL, then brought to memory)
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

            // 3. Event counts + sell-through for upcoming events
            var totalEvents = await events.CountAsync(cancellationToken);
            var upcomingCount = await events.CountAsync(e => e.EndDate >= now, cancellationToken);

            var upcoming = await _context.OrganizerEvents.AsNoTracking()
                .Where(oe => oe.OrganizerId == orgId && oe.Event.EndDate >= now)
                .OrderBy(oe => oe.Event.StartDate)
                .Take(6)
                .Select(oe => new
                {
                    OrganizerEventId = oe.Id,
                    EventId = oe.EventId,
                    Title = oe.Event.Title,
                    StartDate = oe.Event.StartDate,
                    Capacity = oe.Event.TicketTypes.Sum(t => (int?)t.QuantityAvailable) ?? 0
                })
                .ToListAsync(cancellationToken);

            var organizerEventIds = upcoming.Select(u => u.OrganizerEventId).ToList();

            var soldByEvent = await paidOrders
                .Where(o => organizerEventIds.Contains(o.OrganizerEventId))
                .GroupBy(o => o.OrganizerEventId)
                .Select(g => new { OrganizerEventId = g.Key, Sold = g.Sum(o => o.Tickets.Count) })
                .ToDictionaryAsync(x => x.OrganizerEventId, x => x.Sold, cancellationToken);

            var upcomingSales = upcoming.Select(u => new EventSalesDto
            {
                EventId = u.EventId,
                Title = u.Title,
                StartDateTime = u.StartDate,
                TicketsSold = soldByEvent.GetValueOrDefault(u.OrganizerEventId),
                Capacity = u.Capacity
            }).ToList();

            // 4. Ticket type mix
            var mix = await _context.Tickets.AsNoTracking()
                .Where(t => t.TicketOrder.Status == OrderStatus.Paid
                         && _context.OrganizerEvents.Any(oe => oe.OrganizerId == orgId && oe.Id == t.TicketOrder.OrganizerEventId))
                .GroupBy(t => t.TicketType.Name)
                .Select(g => new TicketTypeSalesDto { Name = g.Key, Sold = g.Count() })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync(cancellationToken);

            // 5. Recent orders
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