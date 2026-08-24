using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using UserProfileApp.Models;

namespace UserProfileApp.ViewModels
{
    public class ProfileDetailsViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
        public bool TwoFactorEnabled { get; set; }
        public DateTime AccountCreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }

        public int ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? CoverPhotoUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public string? WebsiteUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? Skills { get; set; }

        public string? TimeZone { get; set; }
        public string? Language { get; set; }
        public int ProfileCompletionPercentage { get; set; }
        public bool IsProfilePublic { get; set; }
        public bool EmailNotifications { get; set; }
        public DateTime ProfileUpdatedAt { get; set; }

        public List<UserActivityLog> ActivityLogs { get; set; } = new();
        public List<UserSession> Sessions { get; set; } = new();
    }

    public class EditProfileViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        [Display(Name = "Professional Headline / Job Title")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        [Display(Name = "About / Bio")]
        public string? Bio { get; set; }

        public string? CurrentProfilePictureUrl { get; set; }
        public string? CurrentCoverPhotoUrl { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Cover Banner")]
        public IFormFile? CoverPhotoFile { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [StringLength(250)]
        [Display(Name = "Street Address")]
        public string? Address { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(100)]
        [Display(Name = "State / Province")]
        public string? State { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        // Social Links
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Portfolio / Website")]
        public string? WebsiteUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid GitHub URL")]
        [Display(Name = "GitHub Profile URL")]
        public string? GitHubUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid LinkedIn URL")]
        [Display(Name = "LinkedIn Profile URL")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid Twitter / X URL")]
        [Display(Name = "Twitter / X URL")]
        public string? TwitterUrl { get; set; }

        [Display(Name = "Technical Skills (comma-separated)")]
        public string? Skills { get; set; }

        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }

        [Display(Name = "Language")]
        public string? Language { get; set; }

        public bool IsProfilePublic { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
    }

    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm new password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me on this device")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
