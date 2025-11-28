using System.ComponentModel.DataAnnotations;
using StoreSystem.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace StoreSystem.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(30)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string FullName => $"{FirstName} {LastName}";
        public string? Role { get; set; }
        public string? GoogleId { get; set; }
    }
}