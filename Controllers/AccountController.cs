using Evently.Constants;
using Evently.DTOs;
using Evently.Interfaces;
using Evently.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evently.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                City = model.City,
                Province = model.Province,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _authService.RegisterAsync(dto);

            if (result.Succeeded)
            {
                TempData["Success"] = "Registration successful. You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterOrganizer()
        {
            return View(new RegisterOrganizerViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOrganizer(RegisterOrganizerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterOrganizerDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,

                OrganizerName = model.OrganizerName,
                Description = model.Description,
                ContactEmail = model.ContactEmail,
                ContactNumber = model.ContactNumber,
                WebsiteUrl = model.WebsiteUrl
            };

            var result = await _authService.RegisterOrganizerAsync(dto);

            if (result.Succeeded)
            {
                TempData["Success"] = "Your organizer account has been created. Sign in to set up your first event.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }

            var result = await _authService.LoginAsync(dto);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                var roles = await _authService.GetRolesAsync(dto.Email);

                if (roles.Contains(AppRoles.Organizer))
                    return RedirectToAction("Dashboard", "Organizer");

                return RedirectToAction("Dashboard", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is temporarily locked. Please try again later."
                );
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password."
                );
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
