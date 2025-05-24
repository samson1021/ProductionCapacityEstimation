using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    public class TaskNotification
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public required string Notification { get; set; }
        public required string Status { get; set; }
        public DateTime Date { get; set; }
        public string URL { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}