namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class ReturnedProductionPostDto
    {
        public required Guid PCEId { get; set; }
        public required string Reason { get; set; }
    }
}
