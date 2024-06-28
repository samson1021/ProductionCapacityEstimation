using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.PCE.Entities
{
    public class PCESchedule
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PCECaseId")]
        public Guid PCECaseId { get; set; }
        public virtual PCECase PCECase { get; set; }

        public DateTime ScheduleDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
