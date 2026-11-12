using Evently.DTOs;
using Evently.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;

namespace Evently.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);

        Task<SignInResult> LoginAsync(LoginDto dto);

        Task LogoutAsync();
    }
}
