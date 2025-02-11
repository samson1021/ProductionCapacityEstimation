using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.Dto.NotificationDto
{
    // public class Notification
    public class NotificationReturnDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }
        public required string Message { get; set; }
        public required string Status { get; set; }
        public required bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string URL { get; set; } = string.Empty;

        public virtual TaskManagment? Task { get; set; }
        public virtual CreateUser? User { get; set; }
    }
}