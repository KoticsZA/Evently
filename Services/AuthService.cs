
using Evently.Constants;
using Evently.DTOs;
using Evently.Interfaces;
using Evently.Models;
using Microsoft.AspNetCore.Identity;

namespace Evently.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            if (!dto.DateOfBirth.HasValue)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Date of birth is required."
                    });
            }

            var email = dto.Email.Trim();

            var user = new AppUser
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                UserName = email,
                DateOfBirth = dto.DateOfBirth.Value,
                Gender = string.IsNullOrWhiteSpace(dto.Gender)
                    ? null
                    : dto.Gender.Trim()
            };

            var result = await _userManager.CreateAsync(
                user,
                dto.Password
            );

            if (!result.Succeeded)
                return result;

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                AppRoles.Attendee
            );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            return IdentityResult.Success;
        }

        public async Task<SignInResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(
                dto.Email.Trim()
            );

            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true
            );
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
