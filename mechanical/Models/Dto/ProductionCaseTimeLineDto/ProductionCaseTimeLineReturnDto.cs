using mechanical.Models.Entities;

namespace mechanical.Models.Dto.ProductionCaseTimeLineDto
{
    public class ProductionCaseTimeLineReturnDto
    {
        public Guid Id { get; set; }
        public required Guid ProductionCaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }

        public virtual CreateUser? User { get; set; }
    }
}
