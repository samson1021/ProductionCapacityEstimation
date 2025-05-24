using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    [Index(nameof(TaskId))]
    public class TaskComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }
        public required string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("TaskId")]
        public virtual TaskManagment? Task { get; set; }
    }
}