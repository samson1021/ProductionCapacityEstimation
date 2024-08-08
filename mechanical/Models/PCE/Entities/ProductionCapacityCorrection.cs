using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class ProductionCapcityCorrection
    {
        public Guid Id { get; set; }
        public Guid PCECaseId { get; set; } = Guid.Empty;
        public Guid ProductionCapacityId { get; set; } = Guid.Empty;
        public Guid EquipmentId { get; set; } = Guid.Empty;
        public string Comment { get; set; } = string.Empty;
        public string CommentedAttribute { get; set; } = string.Empty;
        public Guid CommentedByUserId { get; set; } = Guid.Empty;
        public virtual CreateUser? CommentedByUserIds { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
