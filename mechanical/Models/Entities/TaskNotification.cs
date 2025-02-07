using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    public class TaskNotification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }
        public required string Notification { get; set; }
        public required string Status { get; set; }
        public DateTime Date { get; set; }
        public string? URL { get; set; } = string.Empty;

        public virtual CreateUser? User { get; set; }

    }
}