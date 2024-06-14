using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.Collateral;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class CollateralEstimationFeeDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Case Id")]
        public Guid? CaseId { get; set; }

        [Display(Name = "Created By")]
        public Guid CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Collateral Class")]
        public CollateralClass CollateralClass { get; set; }

        [Display(Name = "Collateral Category")]
        public CollateralCategory CollateralCategory { get; set; }

        [Display(Name = "Unit of Measure")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Display(Name = "Estimation Fee Per Unit")]
        public decimal EstimationFeePerUnit { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Total Fee")]
        public decimal TotalFee { get; set; }

        [Display(Name = "Status")]
        public FeeStatus Status { get; set; }

        [Display(Name = "Rejection Reason")]
        public string? RejectionReason { get; set; }
    }
}

