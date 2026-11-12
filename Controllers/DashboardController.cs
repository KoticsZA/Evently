
using Evently.DTOs.Dashboard;
using Evently.Interfaces;
using Evently.ViewModels.Account;
using Evently.ViewModels.Bookings;
using Evently.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evently.Controllers
{
    [Authorize(Roles = "Attendee")]
    public class DashboardController : Controller
    {
        private readonly IUserDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController( IUserDashboardService dashboardService,ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            try
            {
                var dto = await _dashboardService.GetDashboardAsync(
                    userId,
                    cancellationToken);

                var model = new UserDashboardViewModel
                {
                    FirstName = dto.FirstName,
                    TotalBookings = dto.TotalBookings,
                    UpcomingEventsCount = dto.UpcomingEventsCount,
                    SavedEventsCount = dto.SavedEventsCount,

                    NextEvent = dto.NextEvent == null
                        ? null
                        : MapBooking(dto.NextEvent),

                    UpcomingEvents = dto.UpcomingEvents
                        .Select(MapBooking)
                        .ToList(),

                    RecommendedEvents = dto.RecommendedEvents
                        .Select(MapEvent)
                        .ToList(),

                    SavedEvents = dto.SavedEvents
                        .Select(MapEvent)
                        .ToList()
                };

                return View(model);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Unable to display the attendee dashboard.");

                return View("DashboardError");
            }
        }

        private static EventCardViewModel MapEvent(EventCardDto dto)
        {
            return new EventCardViewModel
            {
                EventId = dto.EventId,
                Title = dto.Title,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                Category = dto.Category,
                City = dto.City,
                Province = dto.Province,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime,
                MinimumTicketPrice = dto.MinimumTicketPrice,
                IsSaved = dto.IsSaved,
                IsSoldOut = dto.IsSoldOut
            };
        }

        private static TicketBookingViewModel MapBooking(TicketBookingDto dto)
        {
            return new TicketBookingViewModel
            {
                TicketOrderId = dto.TicketOrderId,
                BookingReference = dto.BookingReference,
                EventTitle = dto.EventTitle,
                EventCoverImageUrl = dto.EventCoverImageUrl,
                VenueName = dto.VenueName,
                City = dto.City,
                EventStartDateTime = dto.EventStartDateTime,
                EventEndDateTime = dto.EventEndDateTime,
                BookingDate = dto.BookingDate,
                TotalTickets = dto.TotalTickets,
                TotalAmount = dto.TotalAmount,
                BookingStatus = dto.BookingStatus,
                IsUpcoming = dto.EventStartDateTime >= DateTime.UtcNow,

                Tickets = dto.Tickets.Select(t =>
                    new BookedTicketViewModel
                    {
                        TicketNumber = t.TicketNumber,
                        TicketTypeName = t.TicketTypeName,
                        TicketStatus = t.TicketStatus,
                        QRCodeValue = t.QRCodeValue
                    }).ToList()
            };
        }
    }
}
