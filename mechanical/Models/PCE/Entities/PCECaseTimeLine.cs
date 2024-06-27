using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class PCECaseTimeLine
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }

        public virtual PCECase? NewCase { get; set; }
        public virtual CreateUser? User { get; set; }
    }
}
