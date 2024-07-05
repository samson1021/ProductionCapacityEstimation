using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    public class Collateral
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; } 
        public required string Role { get; set; }
        public required MechanicalCollateralCategory Category { get; set; }
        public required MechanicalCollateralType Type { get; set; }

        [RegularExpression(@"^\d{2}-[A-Z]{2}-[A-Z]?\d{5}$")]
        public string? PlateNo { get; set; } 
        public string? ChassisNo { get; set; } 
        public string? EngineMotorNo { get; set; }

        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? CollateralType { get; set; }
        public string? SerialNo { get; set; } 
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        //industry collaterals attribute 
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }

        public string? CurrentStage { get; set; } 
        public string? CurrentStatus { get; set; } 
        public DateTime CreationDate { get; set; }
        public Guid CreatedById {  get; set; }

        //Civil
        public string? BlockNo { get; set; }
        public string? FloorNo { get; set; }
        public string? BuildingStatus { get; set; }
        public string? HouseArea { get; set; }
        public string? Ownership { get; set; }

        //Agriculture
        public string? TypeOfFarming { get; set; }
        public string? PurposeOfTheLand { get; set; }
        public string? PlotOfLand { get; set; }
        public string? LHCNo { get; set; }
        
        public virtual CreateUser? CreatedBy { get; set; }
        public virtual Case? Case { get; set; }
    }
}
