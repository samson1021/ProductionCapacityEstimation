using mechanical.Models.Entities;
using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Models.PCE.Entities;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Entities
{
    public class ProductionCapacity
    {
        public Guid Id { get; set; }
        public required Guid PCECaseId { get; set; }
        
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public ManufacturingSector? Category { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public string? Type { get; set; }
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        public string? ProductionBussinessLicence { get; set; }

        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ProductionType { get; set; }
        public string? SerialNo { get; set; }
        public string? MachineryInstalledPlace { get; set; }

        // private Owned LHC
        public string? LHCNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? Industrialpark { get; set; }

        // Address 
        public string? Region { get; set; }
        public string? Zone { get; set; }
        public string? City { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? ProductDescription { get; set; }

        // plant 
        public string? PlantName { get; set; }
        public string? ObsolescenceStatus { get; set; }
        public string? PlantDepreciationRate { get; set; }
        public DateTime? DateOfInspection { get; set; }
        public DateTime? EndDate { get; set; }

        public string? TradeLicenseNumber { get; set; }
        //public string? NameOfPlant { get; set; }
        public string? OwnerOfMachinery { get; set; }
        public string? CountryOfOrgin { get; set; }
        public string? PlaceOfInspection { get; set; }
        public int? YearOfManifacturing { get; set; }
        public string? PurposeOfCollateral { get; set; }

        public string? CurrentStage { get; set; }
        public string? CurrentStatus { get; set; }
        // public string? Remark { get; set; } = string.Empty;
        
        public Guid? EvaluatorUserID { get; set; }
        public Guid? CheckerUserID { get; set; } // remove
        public DateTime? CreationDate { get; set; }
        public Guid? CreatedById { get; set; }
        // public Guid CreatedById { get; set; }
        
        public virtual CreateUser? CreatedBy { get; set; }
        public virtual PCECase? PCECase { get; set; }
        // public virtual PCECase PCECase { get; set; }

        // public Guid Id { get; set; }
        // public required Guid PCECaseId { get; set; }
        
        // public required string PropertyOwner { get; set; }
        // public required string Role { get; set; }
        // [Display(Name = "Manufacturing Sub-Sector")]
        // public ManufacturingSector? Category { get; set; }
        // [Display(Name = "Manufacturing Sub-Sector")]
        // public string? Type { get; set; }
        // public string? MachineName { get; set; }
        // public string? Purpose { get; set; }
        // public string? ModelNo { get; set; }

        // [Range(1900, 2024)]
        // public int? ManufactureYear { get; set; }
        // public string? InvoiceNo { get; set; }
        // public string? SerialNo { get; set; }
        // public string? MachineryInstalledPlace { get; set; }

        // // private Owned LHC
        // public string? LHCNumber { get; set; }
        // public string? OwnerName { get; set; }
        // public string? Industrialpark { get; set; }
        // public string? ProductionType { get; set; }

        // // Address 
        // public string? Region { get; set; }
        // public string? Zone { get; set; }
        // public string? City { get; set; }
        // public string? SubCity { get; set; }
        // public string? Wereda { get; set; }
        // public string? Kebele { get; set; }
        // public string? HouseNo { get; set; }
        // public string? ProductDescription { get; set; }

        // // plant 
        // public string? PlantName { get; set; }
        // public string? ObsolescenceStatus { get; set; }
        // public string? PlantDepreciationRate { get; set; }
        // public DateTime? DateOfInspection { get; set; }
        // public DateTime? EndDate { get; set; }

        // public string? TradeLicenseNumber { get; set; }
        // public string? OwnerOfMachinery { get; set; }        
        
        // public string? BussinessLicence { get; set; }

        // public string? PlaceOfInspection { get; set; }
        // public string? CountryOfOrgin { get; set; }


        // ///////
        // public string? CurrentStage { get; set; }
        // public string? CurrentStatus { get; set; }
        // public string? Remark { get; set; } = string.Empty;
        
        // public DateTime? CreationDate { get; set; }
        // public Guid? CreatedById { get; set; }
        
        // public virtual CreateUser? CreatedBy { get; set; }
        // public virtual PCECase? PCECase { get; set; }
    }
}
