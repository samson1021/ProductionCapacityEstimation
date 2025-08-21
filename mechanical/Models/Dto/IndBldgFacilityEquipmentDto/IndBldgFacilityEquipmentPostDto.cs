using mechanical.Models.Entities;
using mechanical.Models.Enum;

namespace mechanical.Models.Dto.IndBldgFacilityEquipmentDto
{
    public class IndBldgFacilityEquipmentPostDto
    {
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public required Guid IndBldgFacilityEquipmentCostsId { get; set; }
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


        public string PhysicalAndInstallationAssesment { get; set; } = string.Empty;
        public string OverallSurveyAssesment { get; set; } = string.Empty;
        public string? Remark { get; set; } = string.Empty;
        public double InvoiceValue { get; set; }
        public string Currency { get; set; } = string.Empty;
        public double ExchangeRate { get; set; }

        public virtual IndBldgFacilityEquipmentCosts? IndBldgFacilityEquipmentCosts { get; set; }
    }
}
