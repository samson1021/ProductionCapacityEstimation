using mechanical.Models.Entities;
using mechanical.Models.Enum.ProductionCapcityEstimation;
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
        [Display(Name = "Manufacturing Sun-Sector")]
        public required ManufacturingSector Category { get; set; }
        [Display(Name = "Manufacturing Sun-Sector")]
        public required string Type { get; set; }
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
        //private Owned LHC
        public string? LHCNumber { get; set; }
        public string? OwnerName { get; set; }
        //upload LHC---> file
        //Industrial park 
        public string? Industrialpark { get; set; }
        // upload shade rent agreement -->file

        public string? Region { get; set; }
        public string? Zone { get; set; }
        public string? City { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? ProductDescription { get; set; }
        public string? CurrentStage { get; set; }
        public string? CurrentStatus { get; set; }
        //other for remark
        public string? Remark { get; set; } = string.Empty;      
        public Guid EvaluatorUserID { get; set; }
        public Guid? CheckerUserID { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid CreatedById { get; set; }
        public virtual CreateUser? CreatedBy { get; set; }
        public virtual PCECase? PCECase { get; set; }
    }
}
