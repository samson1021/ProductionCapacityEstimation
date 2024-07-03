namespace mechanical.Models.Entities
{
    public class ProductionCapcityCorrection
    {
        public Guid Id { get; set; }
        public Guid ProductionCaseId { get; set; } = Guid.Empty;
        public Guid ProductionCapacityId { get; set; } = Guid.Empty;
        public Guid EquipmentId { get; set; } = Guid.Empty;
        public string Comment { get; set; } = string.Empty;
        public string CommentedAttribute { get; set; } = string.Empty;
        public Guid CommentedByUserId { get; set; } = Guid.Empty;
        public virtual CreateUser? CommentedByUserIds { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
