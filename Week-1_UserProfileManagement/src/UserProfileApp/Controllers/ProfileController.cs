using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProfileApp.Services;
using UserProfileApp.ViewModels;

namespace UserProfileApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IProfileService profileService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "User profile not found.";
                return RedirectToAction("Login", "Account");
            }

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            int userId = GetCurrentUserId();
            var model = await _profileService.GetProfileForEditAsync(userId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            int userId = GetCurrentUserId();
            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _profileService.UpdateProfileAsync(model, _webHostEnvironment.WebRootPath, GetClientIpAddress());
            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return View(model);
            }

            // Refresh Authentication Cookie Claims
            var updatedProfile = await _profileService.GetProfileByUserIdAsync(userId);
            if (updatedProfile != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, updatedProfile.UserId.ToString()),
                    new Claim(ClaimTypes.Name, updatedProfile.FullName),
                    new Claim(ClaimTypes.Email, updatedProfile.Email),
                    new Claim(ClaimTypes.Role, updatedProfile.Role),
                    new Claim("AvatarUrl", updatedProfile.ProfilePictureUrl ?? "/images/default-avatar.png")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel
            {
                UserId = GetCurrentUserId()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.UserId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _profileService.ChangePasswordAsync(model, GetClientIpAddress());
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Password updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle2FA()
        {
            int userId = GetCurrentUserId();
            var (success, isEnabled, message) = await _profileService.ToggleTwoFactorAsync(userId, GetClientIpAddress());
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminateOtherSessions()
        {
            int userId = GetCurrentUserId();
            var (success, message) = await _profileService.TerminateOtherSessionsAsync(userId, GetClientIpAddress());
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportSummary()
        {
            int userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileByUserIdAsync(userId);
            if (profile == null) return RedirectToAction(nameof(Index));

            return View(profile);
        }
    }
}
