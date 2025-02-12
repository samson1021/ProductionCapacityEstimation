namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class UpdateTaskDto
    {
        public required Guid Id { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }
    }
}

