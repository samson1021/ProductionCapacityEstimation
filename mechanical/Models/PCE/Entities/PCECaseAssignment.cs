﻿using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class PCECaseAssignment
    {
        public Guid Id { get; set; }
        public required Guid ProductionCapacityId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public virtual ProductionCapacity? ProductionCapacity { get; set; }
        public virtual CreateUser? User { get; set; }
    }
}
