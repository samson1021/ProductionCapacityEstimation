﻿namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class PCECaseSchedulePostDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public Guid PCECaseId { get; set; }
    }
}
