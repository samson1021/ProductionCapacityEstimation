using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCECaseId))]
    public class ProductionCapacity
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid PCECaseId { get; set; }
        
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public ManufacturingSector Category { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public required string Type { get; set; }
        public required string MachineName { get; set; }
        public required string CountryOfOrigin { get; set; }
        public required string BusinessLicenseNumber { get; set; }
        // public string TradeLicenseNumber { get; set; }

        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? SerialNo { get; set; }
        public required string MachineryInstalledPlace { get; set; }

        // private Owned LHC
        public string? LHCNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? Industrialpark { get; set; }

        // Address
        public required string Region { get; set; }
        public string? Zone { get; set; }
        public string? City { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? ProductDescription { get; set; }

        public string? CurrentStage { get; set; }
        public string? CurrentStatus { get; set; }
        public string? Remark { get; set; } = string.Empty;

        public string? ProductionType { get; set; }
        public Guid? AssignedEvaluatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;

        [ForeignKey("CreatedById")]
        public virtual User? CreatedBy { get; set; }
        [ForeignKey("AssignedEvaluatorId")]
        public virtual User? AssignedEvaluator { get; set; }
        [ForeignKey("PCECaseId")]
        public virtual PCECase? PCECase { get; set; }
    }
}
