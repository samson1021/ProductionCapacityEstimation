using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.Collateral;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class CollateralEstimationFee
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? PCECaseId { get; set; } 
        [ForeignKey("CaseId")]
        public virtual Case? PCECase { get; set; } 

        public CollateralClass CollateralClass { get; set; }
        public CollateralCategory CollateralCategory { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public decimal EstimationFeePerUnit { get; set; }
        
        public int Quantity { get; set; }
        public decimal TotalFee { get; set; }
        public FeeStatus Status { get; set; }
        public string? RejectionReason { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
