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
using System.Security.Claims;

namespace Evently.Services
{
    public class EventService : IEventService
    {
        private readonly EventlyDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EventService(EventlyDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } 
    

       

            public async Task<EventDiscoveryDto> GetEventsAsync(string userId,CancellationToken cancellationToken = default)
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
                        e.Country == user.Country &&
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
                        e.Country == user.Country &&
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
                    Country = user.Country,
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

    }

}

