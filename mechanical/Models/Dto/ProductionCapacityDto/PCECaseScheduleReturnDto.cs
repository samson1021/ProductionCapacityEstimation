using mechanical.Models.Entities;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class PCECaseScheduleReturnDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid PCECaseId { get; set; }

        public virtual CreateUser? User { get; set; }
    }
}
