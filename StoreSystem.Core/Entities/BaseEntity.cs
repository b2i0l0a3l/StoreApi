using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Core.Entities
{
    public abstract class baseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreateByUserId { get; set; }
        [Required]
        [ForeignKey("CreateByUserId")]
        public ApplicationUser? CreateByUser { get; set; }
        public string? UpdateByUserId { get; set; }
        [Required]
        [ForeignKey("UpdateByUserId")]
        public ApplicationUser? UpdateByUser { get; set; }

    }
}