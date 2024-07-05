using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseTerminateDto
{
    public class PCECaseTerminateReturnDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid PCECaseId { get; set; }

        public virtual CreateUser? User { get; set; }
    }
}
