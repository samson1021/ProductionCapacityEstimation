using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseTerminateDto
{
    public class PCECaseTerminateReturnDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime TerminatedAt { get; set; }
        
        public Guid PCECaseId { get; set; }

        public virtual User? PCECaseOriginator { get; set; }


    }
}
