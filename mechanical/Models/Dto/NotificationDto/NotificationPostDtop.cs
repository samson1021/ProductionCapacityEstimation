using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Dto.NotificationDto
{
    // public class Notification
    public class NotificationPostDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public required string Content { get; set; }
        public required string Type { get; set; }
        public string? Link { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public bool IsSeen { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}