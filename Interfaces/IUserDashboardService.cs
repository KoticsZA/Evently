using Evently.DTOs.Dashboard;

namespace Evently.Interfaces
{
    public interface IUserDashboardService
    {
        Task<UserDashboardDto> GetDashboardAsync(string userId,CancellationToken cancellationToken = default);
    }
}