namespace mechanical.Models.Entities
{
    public class ReturnedProduction
    {
        public Guid Id { get; set; }
        public required Guid PCEId { get; set; }
        public required Guid ReturnedById { get; set; }
        public required string Reason { get; set; }
        public DateTime ReturnedAt { get; set; }

        public CreateUser ReturnedBy { get; set; }
    }
}