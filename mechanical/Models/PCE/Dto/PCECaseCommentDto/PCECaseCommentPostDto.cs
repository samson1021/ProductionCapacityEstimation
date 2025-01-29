namespace mechanical.Models.PCE.Dto.PCECaseCommentDto
{
    public class PCECaseCommentPostDto
    {
        public Guid PCECaseId { get; set; }
        public required string Content { get; set; }
    }
}
