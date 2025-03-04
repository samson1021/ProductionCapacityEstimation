using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.Dto.NotificationDto
{
    public class NotificationReturnDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Content { get; set; }
        public required string Type { get; set; }
        public string Link { get; set; } = string.Empty;
        public required bool IsRead { get; set; }
        public required bool IsSeen { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // public virtual CreateUser? User { get; set; }
    }
}