using mechanical.Models.Entities;

namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class TaskCommentReturnDto
    {
        public Guid Id { get; set; }       
        public Guid CommenterId { get; set; }
        public Guid CommenteeId { get; set; }
        public Guid TaskId { get; set; }
        public required string Comment { get; set; }
         public DateTime CommentDate { get; set; }
       
    }
}
