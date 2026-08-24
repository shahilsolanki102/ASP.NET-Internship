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
            IProfileService _profileService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ProfileController> logger)
        {
            this._profileService = _profileService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "User profile could not be found.";
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
            model.UserId = userId; // Ensure security by matching logged-in user

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _profileService.UpdateProfileAsync(model, _webHostEnvironment.WebRootPath);
            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return View(model);
            }

            // Refresh authentication cookie with new name & avatar if changed
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

            TempData["SuccessMessage"] = "Your profile has been successfully updated!";
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

            var (success, message) = await _profileService.ChangePasswordAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
