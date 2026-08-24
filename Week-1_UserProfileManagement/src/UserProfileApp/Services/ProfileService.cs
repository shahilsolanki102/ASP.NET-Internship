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
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new ProfileDetailsViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccountCreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                ProfileId = user.UserProfile?.Id ?? 0,
                FullName = user.UserProfile?.FullName ?? user.Username,
                PhoneNumber = user.UserProfile?.PhoneNumber,
                Bio = user.UserProfile?.Bio,
                ProfilePictureUrl = user.UserProfile?.ProfilePictureUrl ?? "/images/default-avatar.png",
                DateOfBirth = user.UserProfile?.DateOfBirth,
                Gender = user.UserProfile?.Gender,
                Address = user.UserProfile?.Address,
                City = user.UserProfile?.City,
                State = user.UserProfile?.State,
                Country = user.UserProfile?.Country,
                PostalCode = user.UserProfile?.PostalCode,
                ProfileUpdatedAt = user.UserProfile?.UpdatedAt ?? user.CreatedAt
            };
        }

        public async Task<EditProfileViewModel?> GetProfileForEditAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new EditProfileViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.UserProfile?.FullName ?? user.Username,
                Email = user.Email,
                PhoneNumber = user.UserProfile?.PhoneNumber,
                Bio = user.UserProfile?.Bio,
                CurrentProfilePictureUrl = user.UserProfile?.ProfilePictureUrl ?? "/images/default-avatar.png",
                DateOfBirth = user.UserProfile?.DateOfBirth,
                Gender = user.UserProfile?.Gender,
                Address = user.UserProfile?.Address,
                City = user.UserProfile?.City,
                State = user.UserProfile?.State,
                Country = user.UserProfile?.Country,
                PostalCode = user.UserProfile?.PostalCode
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model, string webRootPath)
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

                // Check if email is being updated and is not already taken by another user
                if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.UserId);
                    if (emailExists)
                    {
                        return (false, "Email address is already in use by another account.");
                    }
                    user.Email = model.Email;
                }

                // Ensure UserProfile exists
                if (user.UserProfile == null)
                {
                    user.UserProfile = new UserProfile
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        ProfilePictureUrl = "/images/default-avatar.png",
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.UserProfiles.Add(user.UserProfile);
                }

                // Handle Profile Picture File Upload
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    var (uploadSuccess, uploadResult) = await ProcessProfilePictureAsync(model.ProfilePictureFile, webRootPath);
                    if (!uploadSuccess)
                    {
                        return (false, uploadResult);
                    }
                    user.UserProfile.ProfilePictureUrl = uploadResult;
                }

                // Update Profile Fields
                user.UserProfile.FullName = model.FullName;
                user.UserProfile.PhoneNumber = model.PhoneNumber;
                user.UserProfile.Bio = model.Bio;
                user.UserProfile.DateOfBirth = model.DateOfBirth;
                user.UserProfile.Gender = model.Gender;
                user.UserProfile.Address = model.Address;
                user.UserProfile.City = model.City;
                user.UserProfile.State = model.State;
                user.UserProfile.Country = model.Country;
                user.UserProfile.PostalCode = model.PostalCode;
                user.UserProfile.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return (true, "Profile updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for User ID {UserId}", model.UserId);
                return (false, "An error occurred while saving your profile. Please try again.");
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    return (false, "The current password you entered is incorrect.");
                }

                user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                return (true, "Password changed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for User ID {UserId}", model.UserId);
                return (false, "An error occurred while changing password.");
            }
        }

        private static async Task<(bool Success, string Result)> ProcessProfilePictureAsync(IFormFile file, string webRootPath)
        {
            // Allowed extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return (false, "Invalid file format. Please upload JPG, PNG, or WebP images only.");
            }

            // Max file size: 2MB
            if (file.Length > 2 * 1024 * 1024)
            {
                return (false, "Image file size exceeds the 2MB limit.");
            }

            // Create uploads directory if not exists
            var uploadsDir = Path.Combine(webRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Generate unique secure filename
            var uniqueFileName = $"profile_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, $"/uploads/profiles/{uniqueFileName}");
        }
    }
}
