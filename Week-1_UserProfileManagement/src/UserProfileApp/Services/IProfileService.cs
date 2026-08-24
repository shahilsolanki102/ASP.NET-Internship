using UserProfileApp.ViewModels;

namespace UserProfileApp.Services
{
    public interface IProfileService
    {
        Task<ProfileDetailsViewModel?> GetProfileByUserIdAsync(int userId);
        Task<EditProfileViewModel?> GetProfileForEditAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model, string webRootPath, string? ipAddress = null);
        Task<(bool Success, string Message)> ChangePasswordAsync(ChangePasswordViewModel model, string? ipAddress = null);
        Task<(bool Success, bool IsEnabled, string Message)> ToggleTwoFactorAsync(int userId, string? ipAddress = null);
        Task<(bool Success, string Message)> LogActivityAsync(int userId, string activityType, string description, string? ipAddress = null);
    }
}
