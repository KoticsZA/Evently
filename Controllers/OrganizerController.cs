using Evently.Constants;
using Evently.Interfaces;
using Evently.ViewModels.Organizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Evently.Controllers
{
    [Authorize(Roles = AppRoles.Organizer)]
    public class OrganizerController : Controller
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        private readonly IOrganizerDashboardService _dashboardService;
        private readonly ILogger<OrganizerController> _logger;

        public OrganizerController(IOrganizerDashboardService dashboardService, ILogger<OrganizerController> logger)
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
                var dto = await _dashboardService.GetDashboardAsync(userId, cancellationToken);

                var viewModel = new OrganizerDashboardViewModel
                {
                    OrganizerName = dto.OrganizerName,
                    IsVerified = dto.IsVerified,
                    TotalRevenue = dto.TotalRevenue,
                    RevenueLast30Days = dto.RevenueLast30Days,
                    AverageOrderValue = dto.AverageOrderValue,
                    TicketsSold = dto.TicketsSold,
                    TotalEvents = dto.TotalEvents,
                    UpcomingEventsCount = dto.UpcomingEventsCount,

                    Charts = new OrganizerChartsViewModel
                    {
                        RevenueLabels = dto.RevenueTrend.Select(p => p.Date.ToString("d MMM", Inv)).ToList(),
                        RevenueValues = dto.RevenueTrend.Select(p => p.Amount).ToList(),

                        EventLabels = dto.UpcomingEventSales.Select(e => e.Title).ToList(),
                        EventSold = dto.UpcomingEventSales.Select(e => e.TicketsSold).ToList(),
                        EventRemaining = dto.UpcomingEventSales.Select(e => Math.Max(0, e.Capacity - e.TicketsSold)).ToList(),

                        TicketTypeLabels = dto.TicketMix.Select(t => t.Name).ToList(),
                        TicketTypeValues = dto.TicketMix.Select(t => t.Sold).ToList()
                    },

                    RecentOrders = dto.RecentOrders.Select(o => new OrganizerOrderRowViewModel
                    {
                        TicketOrderId = o.TicketOrderId,
                        BookingReference = o.BookingReference,
                        EventTitle = o.EventTitle,
                        TotalTickets = o.TotalTickets,
                        TotalAmount = o.TotalAmount,
                        PaymentStatus = o.PaymentStatus,
                        BookingDate = o.BookingDate
                    }).ToList()
                };

                ViewData["FirstName"] = User.FindFirstValue(ClaimTypes.GivenName);

                return View(viewModel);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Organizer profile not found for the signed-in user.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the organizer dashboard.");
                return View("Error");
            }
        }
    }
}
