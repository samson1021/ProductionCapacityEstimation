namespace mechanical.Models.Entities
{
    public class CaseSchedule
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid CaseId { get; set; }


        public virtual Case? Case { get; set; }
        public virtual User? User { get; set; }
    }
}
