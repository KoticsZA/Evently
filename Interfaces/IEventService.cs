using Evently.DTOs.Dashboard;
using Evently.DTOs.Events;
using Evently.ViewModels.Events;

namespace Evently.Interfaces
{
    public interface IEventService
    {
        Task<EventDiscoveryDto> GetEventsAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<int> CreateEventAsync(CreateEventDto dto, string userId, CancellationToken cancellationToken = default);
    }
}
