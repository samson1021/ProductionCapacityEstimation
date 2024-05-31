namespace mechanical.Models.Entities
{
    public class CollateralReestimation
    {
        public Guid Id { get; set; }
        public Guid CollateralId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Collateral? Collateral { get; set; }
    }
}
