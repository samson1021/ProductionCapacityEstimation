namespace mechanical.Models.Entities
{
    public class Reject
    {
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public required Guid RejectedBy { get; set; }
        public required string RejectionComment { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
