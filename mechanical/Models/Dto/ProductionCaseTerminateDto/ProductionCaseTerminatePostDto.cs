namespace mechanical.Models.Dto.ProductionCaseTerminateDto
{
    public class ProductionCaseTerminatePostDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public Guid ProductionCaseId { get; set; }
    }
}
