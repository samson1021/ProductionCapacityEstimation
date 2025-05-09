using mechanical.Models.Entities;
using mechanical.Models.Enum;

namespace mechanical.Models.Dto.IndBldgFacilityEquipmentDto
{
    public class IndBldgFacilityEquipmentReturnDto
    {
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public Guid EvaluatorUserID { get; set; }
        public Guid CheckerUserID { get; set; }
        public Guid? IndBldgFacilityEquipmentCostsId { get; set; }

        public string? MechanicalEqpmntName { get; set; }
        public string IndustrialBuildingMachineryType { get; set; } = string.Empty;
        public string EngineType { get; set; } = string.Empty;
        public string PowerSupply { get; set; } = string.Empty;
        public string MotorPower { get; set; } = string.Empty;
        public string WorkingProductionCapacity { get; set; } = string.Empty;
        public string OtherTechSpec { get; set; } = string.Empty;
        public string MakerCompany { get; set; } = string.Empty;
        public string TechnologyStandard { get; set; } = string.Empty;
        public string MakerPreferenceType { get; set; } = string.Empty;
        public string CurrentEqpmntCondition { get; set; } = string.Empty;
        public string AllocatedPointsRange { get; set; } = string.Empty;


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


        public virtual Collateral? Collateral { get; set; }
    }
}
