namespace mechanical.Models.Entities
{
    public class CaseTimeLine
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }

        public virtual Case? Case { get; set; }
        public virtual User? User { get; set; }
    }
}
