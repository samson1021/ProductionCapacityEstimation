namespace mechanical.Models.PCE.Entities
{
    public class Return
    {
        public Guid Id { get; set; }
        public Guid? PCEId { get; set; }
        // public required Guid? PCEId { get; set; }
        public required Guid ReturnedBy { get; set; }
        public required string Reason { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
