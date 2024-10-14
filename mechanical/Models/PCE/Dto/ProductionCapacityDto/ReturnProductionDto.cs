using System;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Entities;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class ReturnProductionDto
    {

        public Guid Id { get; set; }
        
        [Display(Name = "PCE Case ID")]
        public required Guid PCECaseId { get; set; }
        
        [Display(Name = "Property Owner")]
        public required string PropertyOwner { get; set; }
        
        [Display(Name = "Role")]
        public required string Role { get; set; }
        
        [Display(Name = "Manufacturing Main-Sector")]
        public string? Category { get; set; }
        
        [Display(Name = "Manufacturing Sub-Sector")]
        public string? Type { get; set; }
        
        [Display(Name = "Machine Name")]
        public string? MachineName { get; set; }
        
        [Display(Name = "Purpose")]
        public string? Purpose { get; set; }
        
        [Display(Name = "Model Number")]
        public string? ModelNo { get; set; }
        
        [Display(Name = "Production Business Licence")]
        public string? ProductionBussinessLicence { get; set; }
        
        [Display(Name = "Country of Origin")]
        public string? CountryOfOrgin { get; set; }
        
        [Display(Name = "Manufacture Year")]
        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }
        
        [Display(Name = "Invoice Number")]
        public string? InvoiceNo { get; set; }
        
        [Display(Name = "Serial Number")]
        public string? SerialNo { get; set; }
        
        [Display(Name = "Machinery Installed Place")]
        public string? MachineryInstalledPlace { get; set; }
        
        [Display(Name = "LHC Number")]
        public string? LHCNumber { get; set; }
        
        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }
        
        [Display(Name = "Industrial Park")]
        public string? Industrialpark { get; set; }
        
        [Display(Name = "Region")]
        public string? Region { get; set; }
        
        [Display(Name = "Zone")]
        public string? Zone { get; set; }
        
        [Display(Name = "City")]
        public string? City { get; set; }
        
        [Display(Name = "Sub City")]
        public string? SubCity { get; set; }
        
        [Display(Name = "Wereda")]
        public string? Wereda { get; set; }
        
        [Display(Name = "Kebele")]
        public string? Kebele { get; set; }
        
        [Display(Name = "House Number")]
        public string? HouseNo { get; set; }
        
        [Display(Name = "Product Description")]
        public string? ProductDescription { get; set; }
        
        [Display(Name = "Current Status")]
        public string? CurrentStatus { get; set; }
        
        [Display(Name = "Current Stage")]
        public string? CurrentStage { get; set; }
        
        [Display(Name = "Production Type")]
        public string? ProductionType { get; set; }
        
        [Display(Name = "Remark")]
        public string? Remark { get; set; } = string.Empty;
        
        [Display(Name = "Evaluator User ID")]
        public Guid EvaluatorUserID { get; set; }

        [Display(Name = "Plant Name")]
        public required string PlantName { get; set; }
        
        [Display(Name = "Owner of Plant")]
        public string? OwnerOfPlant { get; set; }
        
        [Display(Name = "Obsolescence Status")]
        public string? ObsolescenceStatus { get; set; }
        
        [Display(Name = "Plant Depreciation Rate")]
        public string? PlantDepreciationRate { get; set; }
        
        [Display(Name = "Date of Inspection")]
        public DateTime? DateOfInspection { get; set; }
        
        [Display(Name = "Creation Date")]
        public DateTime? CreationDate { get; set; }
        
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Assignment Status")]
        public string? AssignmentStatus { get; set; }
        
        [Display(Name = "PCE Case")]
        public virtual PCECase? PCECase { get; set; }
        // public virtual PCECase PCECase { get; set; }
    }
}