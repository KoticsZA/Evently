using Evently.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Evently.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<IdentityResult> RegisterOrganizerAsync(RegisterOrganizerDto dto);
        Task<SignInResult> LoginAsync(LoginDto dto);
        Task<IList<string>> GetRolesAsync(string email);
        Task LogoutAsync();
    }
}
