using mechanical.Models.Entities;

namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class TaskManagmentPostDto
    {
        public required Guid CaseId { get; set; }
        public required Guid AssignedId { get; set; }
        public required Guid CaseOrginatorId { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }
       
    }
}
