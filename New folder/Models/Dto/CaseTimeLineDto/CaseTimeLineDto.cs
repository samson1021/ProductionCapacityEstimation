namespace mechanical.Models.Dto.CaseTimeLineDto
{
    public class CaseTimeLineDto
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }
    }
}
