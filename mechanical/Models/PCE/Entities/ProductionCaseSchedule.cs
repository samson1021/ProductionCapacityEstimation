using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class ProductionCaseSchedule
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid PCECaseId { get; set; }


        public virtual PCECase? PCECase { get; set; }
        public virtual CreateUser? User
        {
            get; set;
        }
    }
}
