
using System;
using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class CollateralEstimationFee
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public virtual Case Case { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public CollateralClass CollateralClass { get; set; }
        public CollateralCategory CollateralCategory { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }

        public decimal EstimationFeePerUnit { get; set; }
        public int Quantity { get; set; }
        public decimal TotalFee { get; set; }
        public FeeStatus Status { get; set; }
    }
}
