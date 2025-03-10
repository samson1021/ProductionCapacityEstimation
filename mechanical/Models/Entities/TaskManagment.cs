using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    public class TaskManagment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid CaseId { get; set; }
        public required Guid AssignedId { get; set; }
        public required Guid CaseOrginatorId { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = "New";
        public bool IsActive { get; set; } = true;

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;
        public DateTime? CompletionDate { get; set; } = null;
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }

        public virtual CreateUser? CaseOrginator { get; set; }
        public virtual CreateUser? Assigned { get; set; }
        public virtual Case? Case { get; set; }
    }
}