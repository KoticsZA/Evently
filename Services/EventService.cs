using Evently.Data;
using Evently.DTOs.Dashboard;
using Evently.DTOs.Events;
using Evently.Interfaces;
using Evently.Models;
using Evently.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Evently.Services
{
    public class EventService : IEventService
    {
        private readonly EventlyDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageStorageService _imageStorage;

        public EventService(EventlyDBContext context, UserManager<AppUser> userManager, IImageStorageService imageStorage)
        {
            _context = context;
            _userManager = userManager;
            _imageStorage = imageStorage;
        }




        public async Task<EventDiscoveryDto> GetEventsAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User could not be found.");

            var today = DateTime.Now;

            var localEvents = await _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.IsPublished && 
                    e.City == user.City &&
                    e.Province == user.Province &&
                    e.StartDate >= today)
                .OrderBy(e => e.StartDate)
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
                    EndDateTime = e.EndDate
                })
                .ToListAsync(cancellationToken);

            var nationalEvents = await _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.IsPublished &&
                    e.Province == user.Province &&
                    e.City != user.City &&
                    e.StartDate >= today)
                .OrderBy(e => e.StartDate)
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
                    EndDateTime = e.EndDate
                })
                .ToListAsync(cancellationToken);

            return new EventDiscoveryDto
            {
                City = user.City,
                Country = user.Province,
                LocalEvents = localEvents,
                NationalEvents = nationalEvents
            };
        }
        public async Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryOptionDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<int> CreateEventAsync(CreateEventDto dto, string userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("An authenticated user is required.");

            if (dto.StartDate <= DateTime.Now)
                throw new ArgumentException("The event start date must be in the future.");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("The event end date must be after the start date.");

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId, cancellationToken);

            if (!categoryExists)
                throw new KeyNotFoundException("The selected event category does not exist.");

            var organizers = await _context.EventOrganizers
                .Where(o => o.OwnerUserId == userId)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            if (organizers.Count == 0)
                throw new KeyNotFoundException("No organizer profile was found for this account.");

            if (organizers.Count > 1)
                throw new InvalidOperationException("Multiple organizer profiles exist. An organizer profile must be selected.");

            string? coverImageUrl = null;
            if (dto.CoverImage is not null)
                coverImageUrl = await _imageStorage.SaveAsync(dto.CoverImage, "events", cancellationToken);

            try
            {
                var newEvent = new Event
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description.Trim(),
                    CoverImageUrl = coverImageUrl,                       

                    VenueName = dto.VenueName.Trim(),
                    StreetAddress = dto.StreetAddress.Trim(),
                    City = dto.City.Trim(),
                    Province = dto.Province.Trim(),
                    Country = dto.Country.Trim(),

                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,

                    CategoryId = dto.CategoryId,

                    CreatedAt = DateTime.UtcNow,
                    IsPublished = false
                };

                var organizerEvent = new OrganizerEvent
                {
                    OrganizerId = organizers[0],
                    Event = newEvent
                };

                newEvent.Organizers.Add(organizerEvent);

                await _context.Events.AddAsync(newEvent, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return newEvent.Id;
            }
            catch
            {
                await _imageStorage.DeleteAsync(coverImageUrl, CancellationToken.None);
                throw;
            }
        }

    }

}

