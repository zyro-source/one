using System.ComponentModel.DataAnnotations;

namespace buildwave.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
        public virtual ICollection<UserSession> Sessions { get; set; }
        = new List<UserSession>();
        public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
    }
}