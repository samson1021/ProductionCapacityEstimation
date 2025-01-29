namespace mechanical.Models.PCE.Dto.PCECaseTimeLineDto
{
    public class PCECaseTimeLinePostDto
    {
        public required Guid PCECaseId { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }
        public Guid? UserId { get; set; }
    }
}
