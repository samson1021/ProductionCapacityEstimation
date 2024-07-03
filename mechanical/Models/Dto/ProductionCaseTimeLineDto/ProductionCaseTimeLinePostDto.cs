namespace mechanical.Models.Dto.ProductionCaseTimeLineDto
{
    public class ProductionCaseTimeLinePostDto
    {
        public required Guid ProductionCaseId { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }
        public Guid? UserId { get; set; }
    }
}
