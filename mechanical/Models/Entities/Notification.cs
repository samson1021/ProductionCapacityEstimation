using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    // public class Notification
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public required string Message { get; set; }

        public required string Type { get; set; }
        public string Link { get; set; } = string.Empty;
        public required bool IsRead { get; set; }

        [Required]
        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual CreateUser? User { get; set; }
    }
}