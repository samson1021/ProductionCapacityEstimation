using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Enum;

namespace mechanical.Models.Entities
{
    [Index(nameof(CollateralId))]
    public class IndBldgFacilityEquipment
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public Guid EvaluatorUserID { get; set; }
        public Guid? CheckerUserID { get; set; }
        public Guid? IndBldgFacilityEquipmentCostsId { get; set; }

        public string? MechanicalEqpmntName { get; set; }
        public IndustrialBuildingMachineryType IndustrialBuildingMachineryType { get; set; }
        public EngineType EngineType { get; set; }
        public PowerSupply PowerSupply { get; set; }
        public string MotorPower { get; set; } = string.Empty;
        public string WorkingProductionCapacity { get; set; } = string.Empty;
        public string OtherTechSpec { get; set; } = string.Empty;
        public string MakerCompany { get; set; } = string.Empty;
        public MachineryTechnologyStandard TechnologyStandard { get; set; }
        public string MakerPreferenceType { get; set; } = string.Empty;
        public EquipmentCondition CurrentEqpmntCondition { get; set; }
        public AllocatedPointRange AllocatedPointsRange { get; set; }

        public string ModelNo { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string EngineNo { get; set; } = string.Empty;
        public string ScaleOfOperation { get; set; } = string.Empty;
        public string NoOfProductionLine { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public string CountryOfOrigin { get; set; } = string.Empty;

        public double MarketShareFactor { get; set; }
        public double DepreciationRate { get; set; }
        public double EqpmntConditionFactor { get; set; }
        public double ReplacementCost { get; set; }
        public double NetEstimationValue { get; set; }

        public string PhysicalAndInstallationAssesment { get; set; } = string.Empty;
        public string OverallSurveyAssesment { get; set; } = string.Empty;
        public string? Remark { get; set; } = string.Empty;
        public double InvoiceValue { get; set; }
        public string Currency { get; set; } = string.Empty;
        public double ExchangeRate { get; set; }

        [ForeignKey("EvaluatorUserID")]
        public virtual User? EvaluatorUser { get; set; }
        [ForeignKey("CheckerUserID")]
        public virtual User? CheckerUser { get; set; }
        [ForeignKey("CollateralId")]
        public virtual Collateral? Collateral { get; set; }
        public virtual IndBldgFacilityEquipmentCosts? IndBldgFacilityEquipmentCosts { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
    }
}
