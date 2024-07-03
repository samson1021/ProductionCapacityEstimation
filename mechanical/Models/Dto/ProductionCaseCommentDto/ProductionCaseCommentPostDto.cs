namespace mechanical.Models.Dto.ProductionCaseCommentDto
{
    public class ProductionCaseCommentPostDto
    {
         public Guid ProductionCaseId { get; set; }
        public required string Content { get; set; }

    }
}
