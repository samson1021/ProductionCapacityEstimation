using mechanical.Models.Entities;

namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class TaskManagmentReturnDto
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid AssignedId { get; set; }
        public required Guid CaseOrginatorId { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;
        public required string TaskStatus { get; set; }

        public required bool IsActive { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }

        public virtual User? CaseOrginator { get; set; }
        public virtual User? Assigned { get; set; }
        public virtual Case? Case { get; set; }
    }
}
