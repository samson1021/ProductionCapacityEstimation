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
        public string SharingReason { get; set; }
        public required string TaskStatus { get; set; }
        public DateTime Deadline { get; set; }
        public required string PriorityType { get; set; }
        public IEnumerable<Guid> SelectedRMs { get; set; }

        // public IEnumerable<CaseReturntDto> Cases { get; set; }
        // public IEnumerable<CreateUser> RMs { get; set; }
        // public IEnumerable<TaskManagmentReturnDto> Tasks { get; set; }
        // public IEnumerable<TaskManagmentReturnDto> AssignedTasks { get; set; }
    }
}