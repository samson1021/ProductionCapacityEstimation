namespace mechanical.Models.Dto.ProductionCaseAssignmentDto
{
    public class ProductionCaseAssignmentDto
    {
        public required Guid ProductionCaseId { get; set; }
        public required Guid UserId { get; set; }
    }
}
