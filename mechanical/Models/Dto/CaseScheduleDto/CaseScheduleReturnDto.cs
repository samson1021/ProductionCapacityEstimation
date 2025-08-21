using mechanical.Models.Entities;

namespace mechanical.Models.Dto.CaseScheduleDto
{
    public class CaseScheduleReturnDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid CaseId { get; set; }

        public virtual User? User { get; set; }
    }
}
