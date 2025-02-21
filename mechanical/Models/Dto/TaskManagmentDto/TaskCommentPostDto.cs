namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class TaskCommentPostDto
    {
        public Guid TaskId { get; set; }       
        public Guid CommenteeId { get; set; }
        public required string Comment { get; set; }
       
    }
}
