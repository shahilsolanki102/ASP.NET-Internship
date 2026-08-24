using UserProfileApp.ViewModels;

namespace UserProfileApp.Services
{
    public interface IProfileService
    {
        Task<ProfileDetailsViewModel?> GetProfileByUserIdAsync(int userId);
        Task<EditProfileViewModel?> GetProfileForEditAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model, string webRootPath);
        Task<(bool Success, string Message)> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}
