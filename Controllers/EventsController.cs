using Evently.Constants;
using Evently.DTOs;
using Evently.DTOs.Events;
using Evently.Interfaces;
using Evently.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Evently.Controllers
{
    public class EventsController : Controller
    {
        private const long MaxCreateRequestBytes = 6 * 1024 * 1024;
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService,ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [Authorize(Roles ="Attendee")]
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

        [HttpGet]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            try
            {
                var model = new CreateEventViewModel();

                await PopulateFormOptionsAsync(model, cancellationToken);

                return View(model);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while loading the event creation form.");

                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        [RequestSizeLimit(MaxCreateRequestBytes)]                       // NEW
        public async Task<IActionResult> Create(CreateEventViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                AskToReselectImage(model);                              // NEW
                await PopulateFormOptionsAsync(model, cancellationToken);
                return View(model);
            }

            ImageUploadDto? cover = null;                               // NEW
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Challenge();

                // NEW: wrap the uploaded file without leaking IFormFile into the service layer
                if (model.CoverImage is { Length: > 0 } file)
                {
                    cover = new ImageUploadDto
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Length = file.Length,
                        Content = file.OpenReadStream()
                    };
                }

                var dto = new CreateEventDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    CoverImage = cover,                                 // CHANGED (was CoverImageUrl = model.CoverImageUrl)

                    VenueName = model.VenueName,
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    Province = model.Province,
                    Country = model.Country,

                    StartDate = model.StartDate,
                    EndDate = model.EndDate,

                    CategoryId = model.CategoryId
                };

                int eventId = await _eventService.CreateEventAsync(dto, userId, cancellationToken);

                TempData["SuccessMessage"] = "Your event has been created successfully.";

                return RedirectToAction("Details", new { id = eventId });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Event creation failed because a related resource was not found.");
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized event creation attempt.");
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Event creation could not be completed due to an invalid organizer state.");
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating an event.");
                ModelState.AddModelError(string.Empty, "We couldn't save your event. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating an event.");
                return View("Error");
            }
            finally
            {
                if (cover is not null)                                  
                    await cover.DisposeAsync();
            }

            AskToReselectImage(model);                                  // NEW
            await PopulateFormOptionsAsync(model, cancellationToken);

            return View(model);
        }

        private void AskToReselectImage(CreateEventViewModel model)
        {
            if (model.CoverImage is { Length: > 0 } && ModelState[nameof(model.CoverImage)]?.Errors.Count is null or 0)
                ModelState.AddModelError(nameof(model.CoverImage), "Your details were kept, but please choose your cover image again.");
        }

        private async Task PopulateFormOptionsAsync(CreateEventViewModel model,CancellationToken cancellationToken)
        {
            var categories = await _eventService.GetCategoriesAsync(
                cancellationToken);

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            model.Provinces = new List<SelectListItem>
            {
                new("Eastern Cape", "Eastern Cape"),
                new("Free State", "Free State"),
                new("Gauteng", "Gauteng"),
                new("KwaZulu-Natal", "KwaZulu-Natal"),
                new("Limpopo", "Limpopo"),
                new("Mpumalanga", "Mpumalanga"),
                new("Northern Cape", "Northern Cape"),
                new("North West", "North West"),
                new("Western Cape", "Western Cape")
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckAccess()
        {
            return Json(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                UserName = User.Identity?.Name,

                Claims = User.Claims.Select(c => new
                {
                    c.Type,
                    c.Value
                }),

                IsOrganizer = User.IsInRole(AppRoles.Organizer)
            });
        }
    }
}