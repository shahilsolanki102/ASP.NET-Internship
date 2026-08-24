using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileApp.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Headline { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(300)]
        public string? ProfilePictureUrl { get; set; }

        [MaxLength(300)]
        public string? CoverPhotoUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(200)]
        public string? WebsiteUrl { get; set; }

        [MaxLength(200)]
        public string? GitHubUrl { get; set; }

        [MaxLength(200)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(200)]
        public string? TwitterUrl { get; set; }

        [MaxLength(500)]
        public string? Skills { get; set; }

        [MaxLength(100)]
        public string? TimeZone { get; set; } = "(GMT+05:30) India Standard Time";

        [MaxLength(50)]
        public string? Language { get; set; } = "English (US)";

        public int ProfileCompletionPercentage { get; set; } = 25;

        public bool IsProfilePublic { get; set; } = true;

        public bool EmailNotifications { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual User? User { get; set; }
    }
}
