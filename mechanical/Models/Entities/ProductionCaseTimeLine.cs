namespace mechanical.Models.Entities
{
    public class ProductionCaseTimeLine
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductionCaseId { get; set; }
        public virtual ProductionCase? ProductionCase { get; set; }
        public virtual CreateUser? User { get; set; }
    }
}
