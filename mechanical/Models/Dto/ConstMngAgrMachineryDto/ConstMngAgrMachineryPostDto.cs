using mechanical.Models.Enum;

namespace mechanical.Models.Dto.ConstMngAgrMachineryDto
{
    public class ConstMngAgrMachineryPostDto
    {
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public string? MechanicalEqpmntName { get; set; }
        public ConstructionMiningAgriculturalMachineryType constructionMiningAgriculturalMachineryType { get; set; }
        public EngineType EngineType { get; set; }
        public PowerSupply PowerSupply { get; set; }
        public NoOfCylinder NoOfCylinder { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public IgnitionSystem IgnitionSystem { get; set; }
        public CoolingSystem CoolingType { get; set; }
        public string EnginePower { get; set; } = string.Empty;
        public string WorkingProductionCapacity { get; set; } = string.Empty;
        public MachineryTechnologyStandard TechnologyStandard { get; set; }
        public string MakerPreferenceType { get; set; } = string.Empty;
        public CabinType CabinType { get; set; }
        public NoOfAxles NumberOfAxle { get; set; }
        public string Color { get; set; } = string.Empty;
        public string MakerCompany { get; set; } = string.Empty;
        public EquipmentCondition CurrentEqpmntCondition { get; set; }
        public AllocatedPointRange AllocatedPointsRange { get; set; }
        public string ModelNo { get; set; } = string.Empty;
        public string EngineNo { get; set; } = string.Empty;
        public string ChassisNo { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string PlateNo { get; set; } = string.Empty;
        public string TDNo { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public string CountryOfOrigin { get; set; } = string.Empty;

        public string PhysicalAndInstallationAssesment { get; set; } = string.Empty;
        public string OverallSurveyAssesment { get; set; } = string.Empty;
        public string? Remark { get; set; } = string.Empty;
        public double InvoiceValue { get; set; }
        public string Currency { get; set; } = string.Empty;
        public double ExchangeRate { get; set; }
    }
}
