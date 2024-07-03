using mechanical.Models.Entities;

namespace mechanical.Models.Dto.CaseTerminateDto
{
    public class CaseTerminateReturnDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid CaseId { get; set; }

        public virtual CreateUser? User { get; set; }
    }
}
