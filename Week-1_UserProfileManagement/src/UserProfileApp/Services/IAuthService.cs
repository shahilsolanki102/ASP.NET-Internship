using UserProfileApp.Models;
using UserProfileApp.ViewModels;

namespace UserProfileApp.Services
{
    public interface IAuthService
    {
        Task<(bool Success, User? User, string Message)> AuthenticateAsync(string email, string password);
        Task<(bool Success, User? User, string Message)> RegisterAsync(RegisterViewModel model);
        Task<User?> GetUserByIdAsync(int id);
    }
}
