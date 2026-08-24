using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileApp.Models
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Device { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Browser { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Location { get; set; }

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public bool IsCurrent { get; set; } = false;

        public virtual User? User { get; set; }
    }
}
