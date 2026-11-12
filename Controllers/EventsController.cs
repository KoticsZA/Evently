using Evently.DTOs.Events;
using Evently.Interfaces;
using Evently.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evently.Controllers
{
    [Authorize(Roles = "Attendee")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService,ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            try
            {
                var dto = await _eventService.GetEventsAsync(
                    userId, cancellationToken);

                var categories = await _eventService.GetCategoriesAsync(
                    cancellationToken);

                var viewModel = new EventDiscoveryViewModel
                {
                    Events = dto.LocalEvents.Select(e => new EventCardViewModel
                    {
                        EventId = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        CoverImageUrl = e.CoverImageUrl,
                        Category = e.Category,
                        City = e.City,
                        Province = e.Province,
                        StartDateTime = e.StartDateTime,
                        EndDateTime = e.EndDateTime,
                        MinimumTicketPrice = e.MinimumTicketPrice,
                        IsSaved = e.IsSaved,
                        IsSoldOut = e.IsSoldOut
                    }).ToList(),

                    OtherRegionEvents = dto.NationalEvents.Select(e => new EventCardViewModel
                    {
                        EventId = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        CoverImageUrl = e.CoverImageUrl,
                        Category = e.Category,
                        City = e.City,
                        Province = e.Province,
                        StartDateTime = e.StartDateTime,
                        EndDateTime = e.EndDateTime,
                        MinimumTicketPrice = e.MinimumTicketPrice,
                        IsSaved = e.IsSaved,
                        IsSoldOut = e.IsSoldOut
                    }).ToList(),

                    TotalCount = dto.LocalEvents.Count + dto.NationalEvents.Count,

                    Categories = categories.Select(c => new FilterOption(
                        c.Id.ToString(),
                        c.Name
                    )).ToList()
                };

                return View(viewModel);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Authenticated user was not found.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while loading event discovery.");

                return View("Error");
            }
        }
    }
}