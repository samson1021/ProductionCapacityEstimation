namespace mechanical.Models.Dto.ProductionCaseCommentDto
{
    public class ProductionCaseCommentReturnDto
    {
        public Guid Id { get; set; }
        public Guid ProductionCaseId { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
