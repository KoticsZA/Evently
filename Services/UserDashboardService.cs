
using Evently.Data;
using Evently.DTOs.Dashboard;
using Evently.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evently.Services
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly EventlyDBContext _context;
        private readonly ILogger<UserDashboardService> _logger;

        public UserDashboardService(
            EventlyDBContext context,
            ILogger<UserDashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDashboardDto> GetDashboardAsync( string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException(
                    "A valid user ID is required.",
                    nameof(userId));
            }

            try
            {
                var now = DateTime.UtcNow;

                var firstName = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => u.FirstName)
                    .SingleOrDefaultAsync(cancellationToken);

                if (firstName == null)
                {
                    throw new KeyNotFoundException(
                        "The requested user could not be found.");
                }

                var orders = await _context.TicketOrders
                    .AsNoTracking()
                    .Where(o => o.UserId == userId && o.Tickets.Any())
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderNumber,
                        o.TotalAmount,
                        o.Status,
                        o.CreatedAt,

                        Tickets = o.Tickets.Select(t => new
                        {
                            t.TicketNumber,
                            t.QRCodeValue,
                            t.Status,

                            TicketTypeName = t.TicketType.Name,

                            EventId = t.TicketType.Event.Id,
                            EventTitle = t.TicketType.Event.Title,
                            EventCoverImageUrl =
                                t.TicketType.Event.CoverImageUrl,
                            VenueName = t.TicketType.Event.VenueName,
                            City = t.TicketType.Event.City,
                            StartDateTime = t.TicketType.Event.StartDate,
                            EndDateTime = t.TicketType.Event.EndDate
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                var bookings = new List<TicketBookingDto>();

                foreach (var order in orders)
                {
                    var eventIds = order.Tickets
                        .Select(t => t.EventId)
                        .Distinct()
                        .ToList();

                    if (eventIds.Count != 1)
                    {
                        throw new InvalidOperationException(
                            $"Order {order.Id} contains tickets for multiple events.");
                    }

                    var eventTicket = order.Tickets.First();

                    var booking = new TicketBookingDto
                    {
                        TicketOrderId = order.Id,
                        BookingReference = order.OrderNumber,
                        EventId = eventTicket.EventId,
                        EventTitle = eventTicket.EventTitle,
                        EventCoverImageUrl =
                            eventTicket.EventCoverImageUrl,
                        VenueName = eventTicket.VenueName,
                        City = eventTicket.City,
                        EventStartDateTime =
                            eventTicket.StartDateTime,
                        EventEndDateTime =
                            eventTicket.EndDateTime,
                        BookingDate = order.CreatedAt,
                        TotalTickets = order.Tickets.Count,
                        TotalAmount = order.TotalAmount,
                        BookingStatus = order.Status.ToString(),

                        Tickets = order.Tickets.Select(t =>
                            new BookedTicketDto
                            {
                                TicketNumber = t.TicketNumber,
                                TicketTypeName = t.TicketTypeName,
                                QRCodeValue = t.QRCodeValue,
                                TicketStatus = t.Status.ToString()
                            }).ToList()
                    };

                    bookings.Add(booking);
                }

                var upcomingBookings = bookings
                    .Where(b => b.EventStartDateTime >= now)
                    .OrderBy(b => b.EventStartDateTime)
                    .ToList();

                var bookedEventIds = bookings
                    .Select(b => b.EventId)
                    .Distinct()
                    .ToList();

                var recommendedEvents = await _context.Events
                    .AsNoTracking()
                    .Where(e =>
                        e.IsPublished &&
                        e.StartDate >= now &&
                        !bookedEventIds.Contains(e.Id))
                    .OrderBy(e => e.StartDate)
                    .Take(8)
                    .Select(e => new EventCardDto
                    {
                        EventId = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        CoverImageUrl = e.CoverImageUrl,
                        Category = e.Category,
                        City = e.City,
                        Province = e.Province,
                        StartDateTime = e.StartDate,
                        EndDateTime = e.EndDate,

                        MinimumTicketPrice = e.TicketTypes
                            .Where(t =>
                                t.QuantitySold < t.QuantityAvailable)
                            .Select(t => (decimal?)t.Price)
                            .Min() ?? 0m,

                        IsSoldOut = !e.TicketTypes.Any(t =>
                            t.QuantitySold < t.QuantityAvailable),

                        IsSaved = false
                    })
                    .ToListAsync(cancellationToken);

                return new UserDashboardDto
                {
                    FirstName = firstName,

                    TotalBookings = bookings
                        .Select(b => b.TicketOrderId)
                        .Distinct()
                        .Count(),

                    UpcomingEventsCount = upcomingBookings.Count,

                    SavedEventsCount = 0,

                    NextEvent = upcomingBookings.FirstOrDefault(),

                    UpcomingEvents = upcomingBookings,

                    RecommendedEvents = recommendedEvents,

                    SavedEvents = new List<EventCardDto>()
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve dashboard data for user {UserId}.",
                    userId);

                throw new InvalidOperationException(
                    "Unable to retrieve dashboard information at this time.",
                    ex);
            }
        }
    }
}
