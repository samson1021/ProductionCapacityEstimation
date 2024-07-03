using mechanical.Models.Entities;

namespace mechanical.Models.Dto.ProductionCaseTerminateDto
{
    public class ProductionCaseTerminateReturnDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid ProductionCaseId { get; set; } = Guid.NewGuid();
        public virtual CreateUser? User { get; set; }
    }
}
