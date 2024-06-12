using System;
using mechanical.Models.Enum;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class ProductionCapacityScheduleDto
    {
        public Guid Id { get; set; }
        public Guid ProductionCapacityEstimationId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}