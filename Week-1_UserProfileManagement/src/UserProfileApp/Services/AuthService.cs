using Microsoft.EntityFrameworkCore;
using UserProfileApp.Data;
using UserProfileApp.Models;
using UserProfileApp.ViewModels;

namespace UserProfileApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, User? User, string Message)> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());

            if (user == null)
            {
                return (false, null, "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return (false, null, "This account has been deactivated.");
            }

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return (false, null, "Invalid email or password.");
            }

            // Update LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, user, "Login successful.");
        }

        public async Task<(bool Success, User? User, string Message)> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Check if username exists
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower().Trim()))
                {
                    return (false, null, "Username is already taken.");
                }

                // Check if email exists
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim()))
                {
                    return (false, null, "Email is already registered.");
                }

                var user = new User
                {
                    Username = model.Username.Trim(),
                    Email = model.Email.Trim().ToLower(),
                    PasswordHash = PasswordHelper.HashPassword(model.Password),
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile
                    {
                        FullName = model.FullName.Trim(),
                        ProfilePictureUrl = "/images/default-avatar.png",
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, user, "Account registered successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", model.Email);
                return (false, null, "An error occurred during registration.");
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
