namespace mechanical.Models.PCE.Entities
{
    public class ProductionReestimation
    {
        public Guid Id { get; set; }
        public Guid PCECaseId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ProductionCapacity? ProductionCapacity { get; set; }
    }
}
