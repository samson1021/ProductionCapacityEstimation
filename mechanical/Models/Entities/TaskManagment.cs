using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    public class TaskManagment
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid AssignedId { get; set; }
        public required Guid CaseOrginatorId { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;
      
        public required string TaskStatus { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; } = null;
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }

        public virtual CreateUser? CaseOrginator { get; set; }
        public virtual CreateUser? Assigned { get; set; }
        public virtual Case? Case { get; set; }
    }
}