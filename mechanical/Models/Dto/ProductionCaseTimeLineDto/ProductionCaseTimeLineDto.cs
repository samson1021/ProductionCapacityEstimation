namespace mechanical.Models.Dto.ProductionCaseTimeLineDto
{
    public class ProductionCaseTimeLineDto
    {
        public Guid Id { get; set; }
        public required Guid ProductionCaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }
    }
}
