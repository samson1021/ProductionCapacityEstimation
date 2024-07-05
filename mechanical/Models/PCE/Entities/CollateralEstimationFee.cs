using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;
using mechanical.Models.PCE.Enum.Collateral;

namespace mechanical.Models.PCE.Entities
{
    public class CollateralEstimationFee
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PCEEId")]
        public Guid? PCEEId { get; set; } 
        public virtual PCEEvaluation? PCEE { get; set; } 

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
