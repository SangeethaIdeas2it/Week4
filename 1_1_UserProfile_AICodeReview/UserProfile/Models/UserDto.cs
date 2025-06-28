using System.ComponentModel.DataAnnotations;

namespace UserProfile.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
    }
} 