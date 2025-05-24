using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseId))]
    [Index(nameof(UserId))]
    public class CaseSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid CaseId { get; set; }

        [ForeignKey("CaseId")]
        public virtual Case? Case { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
