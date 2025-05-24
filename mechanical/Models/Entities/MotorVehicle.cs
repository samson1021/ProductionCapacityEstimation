using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Enum;
namespace mechanical.Models.Entities
{
    [Index(nameof(CollateralId))]
    public class MotorVehicle
    {
        [Key]
        public Guid Id { get; set; }
        public string? MechanicalEqpmntName { get; set; }
        public required Guid CollateralId { get; set; }
        public Guid EvaluatorUserID { get; set; }
        public Guid? CheckerUserID { get; set; }
        public EngineType EngineType { get; set; }
        public NoOfCylinder NoOfCylinder { get; set; } 
        public TransmissionType TransmissionType { get; set; }
        public CoolingSystem coolingSystem { get; set; } 
        public string EnginePower { get; set; } = string.Empty;
        public string LoadingCapacity { get; set; } = string.Empty;
        public BodyType BodyType { get; set; } 
        public CabinType CabinType { get; set; } 
        public NoOfAxles NumberOfAxle { get; set; }
        public string Color { get; set; } = string.Empty;
        public MotorVehicleMake MotorVehicleMake { get; set; } 
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
    }
}
