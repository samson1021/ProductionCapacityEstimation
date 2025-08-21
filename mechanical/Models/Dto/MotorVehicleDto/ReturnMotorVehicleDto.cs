using mechanical.Models.Entities;
using mechanical.Models.Enum;

namespace mechanical.Models.Dto.MotorVehicleDto
{
    public class ReturnMotorVehicleDto
    {
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public Guid EvaluatorUserID { get; set; }
        public Guid CheckerUserID { get; set; }
        public string? MechanicalEqpmntName { get; set; }
        public string EngineType { get; set; } = string.Empty;
        public string NoOfCylinder { get; set; } = string.Empty;
        public string TransmissionType { get; set; } = string.Empty;
        public string coolingSystem { get; set; } = string.Empty;
        public string EnginePower { get; set; } = string.Empty;
        public string LoadingCapacity { get; set; } = string.Empty;
        public string BodyType { get; set; } = string.Empty;
        public string CabinType { get; set; } = string.Empty;
        public string NumberOfAxle { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string MotorVehicleMake { get; set; } = string.Empty;
        public string CurrentEqpmntCondition { get; set; } = string.Empty;
        public string AllocatedPointsRange { get; set; } = string.Empty;
        public string ModelNo { get; set; } = string.Empty;
        public string EngineNo { get; set; } = string.Empty;
        public string ChassisNo { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string PlateNo { get; set; } = string.Empty;
        public string TDNo { get; set; } = string.Empty;
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

        public Collateral Collaterial { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
    }
}
