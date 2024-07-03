namespace mechanical.Models.Entities
{
    public class ProductionCapacityReestimation
    {
        public Guid Id { get; set; }
        public Guid ProductionCapacityId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ProductionCapacity? ProductionCapacity { get; set; }
    }
}
