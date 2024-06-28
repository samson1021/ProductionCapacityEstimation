using System;

namespace mechanical.Models.PCE.Dto.PCEDto
{
    public class PCEScheduleDto
    {
        public Guid Id { get; set; }
        public Guid PCEId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}