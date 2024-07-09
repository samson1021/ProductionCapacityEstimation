namespace mechanical.Models.Dto.CaseCommentDto
{
    public class CaseCommentReturnDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid AuthorId { get; set; }
        public  string? AuthorName { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
