using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Dto.NotificationDto
{
    // public class Notification
    public class NotificationPostDto
    {
        public Guid UserId { get; set; }
        public required string Message { get; set; }
        public required string Type { get; set; }
        public string? Link { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required bool IsRead { get; set; }
    }
}