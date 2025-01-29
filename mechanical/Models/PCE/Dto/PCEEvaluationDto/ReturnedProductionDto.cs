using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class ReturnedProductionDto
    {
        public Guid Id { get; set; }
        public required Guid PCEId { get; set; }
        public required CreateUser ReturnedBy { get; set; }
        public required string Reason { get; set; }
        public DateTime ReturnedAt { get; set; }
    }
}