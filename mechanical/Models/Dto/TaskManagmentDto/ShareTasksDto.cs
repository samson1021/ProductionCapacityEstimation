using System;
using System.Collections.Generic;

using mechanical.Models.Entities;
using mechanical.Models.Dto.CaseDto;

namespace mechanical.Models.Dto.TaskManagmentDto
{
    public class ShareTasksDto
    {
        public required Guid CaseId { get; set; }
        public required string TaskName { get; set; }
        public List<string>? TaskNames { get; set; } // List of TaskNames

        public string? SharingReason { get; set; }
        public DateTime Deadline { get; set; }
        public required string TaskPriority { get; set; }
        public IEnumerable<Guid> SelectedRMs { get; set; }
    }
}