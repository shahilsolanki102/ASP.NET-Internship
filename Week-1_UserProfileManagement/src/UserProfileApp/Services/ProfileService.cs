using Microsoft.EntityFrameworkCore;
using UserProfileApp.Data;
using UserProfileApp.Models;
using UserProfileApp.ViewModels;

namespace UserProfileApp.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(ApplicationDbContext context, ILogger<ProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProfileDetailsViewModel?> GetProfileByUserIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.ActivityLogs.OrderByDescending(a => a.CreatedAt).Take(8))
                .Include(u => u.Sessions.OrderByDescending(s => s.LastActive))
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var profile = user.UserProfile;

            return new ProfileDetailsViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AccountCreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                LastLoginIp = user.LastLoginIp,

                ProfileId = profile?.Id ?? 0,
                FullName = profile?.FullName ?? user.Username,
                Headline = profile?.Headline ?? "Software Developer / Professional",
                PhoneNumber = profile?.PhoneNumber,
                Bio = profile?.Bio,
                ProfilePictureUrl = profile?.ProfilePictureUrl ?? "/images/default-avatar.png",
                CoverPhotoUrl = profile?.CoverPhotoUrl ?? "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1200&auto=format&fit=crop&q=80",
                DateOfBirth = profile?.DateOfBirth,
                Gender = profile?.Gender,
                Address = profile?.Address,
                City = profile?.City,
                State = profile?.State,
                Country = profile?.Country,
                PostalCode = profile?.PostalCode,

                WebsiteUrl = profile?.WebsiteUrl,
                GitHubUrl = profile?.GitHubUrl,
                LinkedInUrl = profile?.LinkedInUrl,
                TwitterUrl = profile?.TwitterUrl,
                Skills = profile?.Skills,

                TimeZone = profile?.TimeZone ?? "(GMT+05:30) India Standard Time",
                Language = profile?.Language ?? "English (US)",
                ProfileCompletionPercentage = profile != null ? CalculateProfileCompletion(profile) : 25,
                IsProfilePublic = profile?.IsProfilePublic ?? true,
                EmailNotifications = profile?.EmailNotifications ?? true,
                ProfileUpdatedAt = profile?.UpdatedAt ?? user.CreatedAt,

                ActivityLogs = user.ActivityLogs.ToList(),
                Sessions = user.Sessions.ToList()
            };
        }

        public async Task<EditProfileViewModel?> GetProfileForEditAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var profile = user.UserProfile;

            return new EditProfileViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = profile?.FullName ?? user.Username,
                Headline = profile?.Headline,
                Email = user.Email,
                PhoneNumber = profile?.PhoneNumber,
                Bio = profile?.Bio,
                CurrentProfilePictureUrl = profile?.ProfilePictureUrl ?? "/images/default-avatar.png",
                CurrentCoverPhotoUrl = profile?.CoverPhotoUrl ?? "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1200&auto=format&fit=crop&q=80",
                DateOfBirth = profile?.DateOfBirth,
                Gender = profile?.Gender,
                Address = profile?.Address,
                City = profile?.City,
                State = profile?.State,
                Country = profile?.Country,
                PostalCode = profile?.PostalCode,
                WebsiteUrl = profile?.WebsiteUrl,
                GitHubUrl = profile?.GitHubUrl,
                LinkedInUrl = profile?.LinkedInUrl,
                TwitterUrl = profile?.TwitterUrl,
                Skills = profile?.Skills,
                TimeZone = profile?.TimeZone ?? "(GMT+05:30) India Standard Time",
                Language = profile?.Language ?? "English (US)",
                IsProfilePublic = profile?.IsProfilePublic ?? true,
                EmailNotifications = profile?.EmailNotifications ?? true
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model, string webRootPath, string? ipAddress = null)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefaultAsync(u => u.Id == model.UserId);

                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.UserId);
                    if (emailExists)
                    {
                        return (false, "This email address is already in use by another account.");
                    }
                    user.Email = model.Email;
                }

                if (user.UserProfile == null)
                {
                    user.UserProfile = new UserProfile { UserId = user.Id, FullName = model.FullName };
                    _context.UserProfiles.Add(user.UserProfile);
                }

                // Profile Avatar Upload
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    var (uploadSuccess, uploadResult) = await ProcessImageUploadAsync(model.ProfilePictureFile, webRootPath, "avatars");
                    if (!uploadSuccess) return (false, uploadResult);
                    user.UserProfile.ProfilePictureUrl = uploadResult;
                }

                // Cover Banner Upload
                if (model.CoverPhotoFile != null && model.CoverPhotoFile.Length > 0)
                {
                    var (uploadSuccess, uploadResult) = await ProcessImageUploadAsync(model.CoverPhotoFile, webRootPath, "covers");
                    if (!uploadSuccess) return (false, uploadResult);
                    user.UserProfile.CoverPhotoUrl = uploadResult;
                }

                // Update Fields
                user.UserProfile.FullName = model.FullName;
                user.UserProfile.Headline = model.Headline;
                user.UserProfile.PhoneNumber = model.PhoneNumber;
                user.UserProfile.Bio = model.Bio;
                user.UserProfile.DateOfBirth = model.DateOfBirth;
                user.UserProfile.Gender = model.Gender;
                user.UserProfile.Address = model.Address;
                user.UserProfile.City = model.City;
                user.UserProfile.State = model.State;
                user.UserProfile.Country = model.Country;
                user.UserProfile.PostalCode = model.PostalCode;

                user.UserProfile.WebsiteUrl = model.WebsiteUrl;
                user.UserProfile.GitHubUrl = model.GitHubUrl;
                user.UserProfile.LinkedInUrl = model.LinkedInUrl;
                user.UserProfile.TwitterUrl = model.TwitterUrl;
                user.UserProfile.Skills = model.Skills;

                user.UserProfile.TimeZone = model.TimeZone ?? "(GMT+05:30) India Standard Time";
                user.UserProfile.Language = model.Language ?? "English (US)";
                user.UserProfile.IsProfilePublic = model.IsProfilePublic;
                user.UserProfile.EmailNotifications = model.EmailNotifications;

                user.UserProfile.ProfileCompletionPercentage = CalculateProfileCompletion(user.UserProfile);
                user.UserProfile.UpdatedAt = DateTime.UtcNow;

                // Log Activity
                _context.UserActivityLogs.Add(new UserActivityLog
                {
                    UserId = user.Id,
                    ActivityType = "Profile",
                    Description = "Profile information updated",
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                return (true, "Profile updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for User ID {UserId}", model.UserId);
                return (false, "An error occurred while saving your profile changes.");
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(ChangePasswordViewModel model, string? ipAddress = null)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null) return (false, "User not found.");

                if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    return (false, "The current password entered is incorrect.");
                }

                user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);

                // Log Activity
                _context.UserActivityLogs.Add(new UserActivityLog
                {
                    UserId = user.Id,
                    ActivityType = "Security",
                    Description = "Account password changed successfully",
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                return (true, "Your password has been changed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for User ID {UserId}", model.UserId);
                return (false, "An error occurred while changing password.");
            }
        }

        public async Task<(bool Success, bool IsEnabled, string Message)> ToggleTwoFactorAsync(int userId, string? ipAddress = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return (false, false, "User not found.");

            user.TwoFactorEnabled = !user.TwoFactorEnabled;

            _context.UserActivityLogs.Add(new UserActivityLog
            {
                UserId = user.Id,
                ActivityType = "Security",
                Description = user.TwoFactorEnabled ? "Two-Factor Authentication enabled" : "Two-Factor Authentication disabled",
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return (true, user.TwoFactorEnabled, user.TwoFactorEnabled ? "Two-Factor Authentication is now ENABLED." : "Two-Factor Authentication is now DISABLED.");
        }

        public async Task<(bool Success, string Message)> TerminateOtherSessionsAsync(int userId, string? ipAddress = null)
        {
            try
            {
                var otherSessions = await _context.UserSessions
                    .Where(s => s.UserId == userId && !s.IsCurrent)
                    .ToListAsync();

                if (otherSessions.Any())
                {
                    _context.UserSessions.RemoveRange(otherSessions);
                    
                    _context.UserActivityLogs.Add(new UserActivityLog
                    {
                        UserId = userId,
                        ActivityType = "Security",
                        Description = $"Terminated {otherSessions.Count} active sessions across other devices",
                        IpAddress = ipAddress,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                }

                return (true, "All other sessions have been successfully logged out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating sessions for user {UserId}", userId);
                return (false, "An error occurred while terminating sessions.");
            }
        }

        public async Task<(bool Success, string Message)> LogActivityAsync(int userId, string activityType, string description, string? ipAddress = null)
        {
            try
            {
                _context.UserActivityLogs.Add(new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = activityType,
                    Description = description,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return (true, "Activity logged.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity");
                return (false, ex.Message);
            }
        }

        private static int CalculateProfileCompletion(UserProfile p)
        {
            int score = 20;
            if (!string.IsNullOrWhiteSpace(p.FullName)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.Headline)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.PhoneNumber)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.Bio)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.ProfilePictureUrl) && !p.ProfilePictureUrl.Contains("default")) score += 15;
            if (!string.IsNullOrWhiteSpace(p.City) || !string.IsNullOrWhiteSpace(p.Country)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.Skills)) score += 10;
            if (!string.IsNullOrWhiteSpace(p.GitHubUrl) || !string.IsNullOrWhiteSpace(p.LinkedInUrl)) score += 5;

            return Math.Min(score, 100);
        }

        private static async Task<(bool Success, string Result)> ProcessImageUploadAsync(IFormFile file, string webRootPath, string folder)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowed.Contains(ext))
            {
                return (false, "Invalid format. Supported: JPG, PNG, WebP.");
            }

            if (file.Length > 3 * 1024 * 1024)
            {
                return (false, "File size exceeds the 3MB limit.");
            }

            var dir = Path.Combine(webRootPath, "uploads", folder);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var fileName = $"{folder}_{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(dir, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, $"/uploads/{folder}/{fileName}");
        }
    }
}
