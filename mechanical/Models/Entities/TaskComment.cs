using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    public class TaskComment
    {
        public Guid Id { get; set; }
        public Guid CommenterId { get; set; }
        public Guid CommenteeId { get; set; }
        public Guid TaskId { get; set; }
        public required string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public virtual CreateUser? User { get; set; }
        public virtual TaskManagment? Task { get; set; }


    }
}