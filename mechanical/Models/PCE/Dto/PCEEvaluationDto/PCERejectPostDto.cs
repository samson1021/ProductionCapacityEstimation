namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCERejectPostDto
    {
        public required Guid PCEId { get; set; }
        public required string RejectionComment { get; set; }
    }
}
