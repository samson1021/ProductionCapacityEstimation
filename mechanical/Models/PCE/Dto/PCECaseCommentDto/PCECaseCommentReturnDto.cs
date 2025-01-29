namespace mechanical.Models.PCE.Dto.PCECaseCommentDto
{
    public class PCECaseCommentReturnDto
    {
        public Guid Id { get; set; }
        public Guid PCECaseId { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
