using Evently.DTOs;
using Evently.DTOs.OrganizerDashboard;

namespace Evently.Interfaces
{
    public interface IOrganizerDashboardService
    {
        Task<OrganizerDashboardDto> GetDashboardAsync(string userId, CancellationToken cancellationToken);
    }
}
