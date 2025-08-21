using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCECaseId))]
    public class PCECaseSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid PCECaseId { get; set; }

        [ForeignKey("PCECaseId")]
        public virtual PCECase? PCECase { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
