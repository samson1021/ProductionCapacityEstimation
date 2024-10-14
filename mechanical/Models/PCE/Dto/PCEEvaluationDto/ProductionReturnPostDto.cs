namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class ProductionReturnPostDto
    {
        public required Guid PCEId { get; set; }
        public required string RejectionComment { get; set; }
        // public required string ReturnComment { get; set; }
    }
}
