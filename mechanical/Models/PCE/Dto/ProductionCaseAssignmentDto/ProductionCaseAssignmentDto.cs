namespace mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto
{
    public class ProductionCaseAssignmentDto
    {
        public required Guid PCECaseId { get; set; }
        public required Guid UserId { get; set; }
    }
}
