using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class ProductionReturnDto
    {
        public Guid Id { get; set; }
        public required Guid PCEId { get; set; }
        public required Guid RejectedBy { get; set; }
        public required CreateUser ReturnedBy { get; set; }
        public required string RejectionComment { get; set; }
        // public required string ReturnComment { get; set; }
        public DateTime CreationDate { get; set; }
    }
}