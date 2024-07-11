using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class ProductionCaseAssignment
    {
        public Guid Id { get; set; }
        public Guid? ProductionCapacityId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;


        public virtual ProductionCapacity? ProductionCapacity { get; set; }
        public virtual CreateUser? User { get; set; }
    }
}
