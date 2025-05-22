using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public required string Content { get; set; } = string.Empty;

        public required string Type { get; set; }
        public string? Link { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public bool IsSeen { get; set; } = false;
        public bool IsArchived { get; set; } = false;

        [Required]
        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}


