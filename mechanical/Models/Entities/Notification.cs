using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    // public class Notification
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Message { get; set; }
        public required string Status { get; set; }
        public required bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string URL { get; set; } = string.Empty;

        public virtual CreateUser? User { get; set; }
    }
}