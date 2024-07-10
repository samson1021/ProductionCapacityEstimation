namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEReturnPostDto
    {
        public Guid? PCEId { get; set; }
        // public required Guid? PCEId { get; set; }
        public required string Reason { get; set; }
    }
}
