using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Dto.NotificationDto
{
    // public class Notification
    public class NotificationPostDto
    {
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }
        public required string Message { get; set; }
        public required string Status { get; set; } = "New";
        public required bool IsRead { get; set; } = false;
        public string URL { get; set; } = string.Empty;
    }
}