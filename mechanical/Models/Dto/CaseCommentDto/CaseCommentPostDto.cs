namespace mechanical.Models.Dto.CaseCommentDto
{
    public class CaseCommentPostDto
    {
        public Guid CaseId { get; set; }
        public required string Content { get; set; }

    }
}
