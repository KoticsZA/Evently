using Evently.Constants;
using Evently.Data;
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
        private readonly EventlyDBContext _db;          
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,EventlyDBContext db,ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var (result, _) = await CreateUserInRoleAsync(dto, AppRoles.Attendee);
            return result;
        }

        public async Task<IdentityResult> RegisterOrganizerAsync(RegisterOrganizerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OrganizerName))
                return Failed("Organisation name is required.");

            var (result, user) = await CreateUserInRoleAsync(dto, AppRoles.Organizer);

            if (!result.Succeeded || user is null)
                return result;

            try
            {
                _db.EventOrganizers.Add(new EventOrganizer      
                {
                    Name = dto.OrganizerName.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    ContactEmail = Clean(dto.ContactEmail) ?? user.Email,
                    ContactNumber = Clean(dto.ContactNumber),
                    WebsiteUrl = Clean(dto.WebsiteUrl),
                    OwnerUserId = user.Id,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Creating the organizer profile for {Email} failed; removing the user.", user.Email);

                await _userManager.DeleteAsync(user);
                return Failed("We couldn't create your organizer profile. Please try again.");
            }
        }

        public async Task<SignInResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email.Trim());

            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true
            );
        }

        public async Task<IList<string>> GetRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());
            return user is null ? new List<string>() : await _userManager.GetRolesAsync(user);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        private async Task<(IdentityResult Result, AppUser? User)> CreateUserInRoleAsync(RegisterDto dto, string role)
        {
            if (!dto.DateOfBirth.HasValue)
                return (Failed("Date of birth is required."), null);

            var email = dto.Email.Trim();

            var user = new AppUser
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                UserName = email,
                DateOfBirth = dto.DateOfBirth.Value,
                Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim()
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (result, null);

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return (roleResult, null);
            }

            return (IdentityResult.Success, user);
        }

        private static IdentityResult Failed(string description) =>
            IdentityResult.Failed(new IdentityError { Description = description });

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
