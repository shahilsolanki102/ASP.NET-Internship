using System.ComponentModel.DataAnnotations;

namespace OrderManagementApp.Models
{
    public enum CustomerTier
    {
        Standard,
        VIP,
        Enterprise
    }

    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public CustomerTier Tier { get; set; } = CustomerTier.Standard;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
    }
}
