namespace mechanical.Models.Entities
{
    public class TaskAssignment
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public required Guid UserId { get; set; }

        public DateTime AssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public virtual Case? Case { get; set; }
        public virtual TaskAssignment? Task { get; set; }
        public virtual CreateUser? AssignedRM { get; set; }
    }
}
