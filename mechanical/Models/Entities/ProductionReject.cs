namespace mechanical.Models.Entities
{
    public class ProductionReject
    {
        public Guid Id { get; set; }
        public required Guid ProductionCapacityId { get; set; }
        public required Guid RejectedBy { get; set; }
        public required string RejectionComment { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
