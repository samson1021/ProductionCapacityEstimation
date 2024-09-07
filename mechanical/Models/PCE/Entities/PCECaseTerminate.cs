using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class PCECaseTerminate
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        // public string? Status { get; set; }
        public string? CurrentStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid RMUserId { get; set; }
        public Guid PCECaseId { get; set; }
        public virtual PCECase? PCECase { get; set; }
        public virtual CreateUser? RMUser { get; set; }
    }
}
