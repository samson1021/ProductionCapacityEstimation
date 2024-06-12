using System;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class CollateralEstimationFeeDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public CollateralClass CollateralClass { get; set; }
        public CollateralCategory CollateralCategory { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }

        public decimal EstimationFeePerUnit { get; set; }
        public int Quantity { get; set; }
        public decimal TotalFee { get; set; }
        public FeeStatus Status { get; set; }
    }
}
