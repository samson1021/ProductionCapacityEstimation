using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class ProductionCapacitySchedule
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("ProductionCapacityEstimation")]
        public Guid ProductionCapacityEstimationId { get; set; }
        public virtual ProductionCapacityEstimation ProductionCapacityEstimation { get; set; }

        public DateTime ScheduleDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
