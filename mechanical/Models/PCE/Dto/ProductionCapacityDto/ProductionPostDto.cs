using mechanical.Models.PCE.Enum.ProductionCapacity;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class ProductionPostDto
    {    
        [Display(Name = "PCE Case ID")]
        public required Guid PCECaseId { get; set; }
        
        [Display(Name = "Property Owner")]
        public required string PropertyOwner { get; set; }
        
        [Display(Name = "Role")]
        public required string Role { get; set; }
        
        [Display(Name = "Manufacturing Main-Sector")]
        public string Category { get; set; }
        
        [Display(Name = "Manufacturing Sub-Sector")]
        public string Type { get; set; }
        
        [Display(Name = "Machine Name")]
        public required string MachineName { get; set; }     
        
        [Display(Name = "Country of Origin")]
        public string CountryOfOrgin { get; set; }

        [Display(Name = "Business License Number")]
        public string BusinessLicenseNumber { get; set; }  
        // [Display(Name = "Trade License Number")]
        // public string TradeLicenseNumber { get; set; }
        
        [Display(Name = "Purpose")]
        public required string Purpose { get; set; }
        
        [Display(Name = "Model Number")]
        public required string ModelNo { get; set; }  
        
        [Display(Name = "Manufacture Year")]
        [Range(1900, 2024)]
        public required int ManufactureYear { get; set; }
        
        [Display(Name = "Invoice Number")]
        public required string InvoiceNo { get; set; }
        
        [Display(Name = "Serial Number")]
        public required string SerialNo { get; set; }
        
        [Display(Name = "Machinery Installed Place")]
        public string MachineryInstalledPlace { get; set; }
        
        [Display(Name = "LHC Number")]
        public string? LHCNumber { get; set; }
        
        [Display(Name = "LHC Owner Name")]
        public string? OwnerName { get; set; }
        
        [Display(Name = "Industrial Park")]
        public string? Industrialpark { get; set; }
        
        [Display(Name = "Region")]
        public string Region { get; set; }
        
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
        public string ProductDescription { get; set; }       
        
        [Display(Name = "Remark")]
        public string? Remark { get; set; } = string.Empty;        
        
        [Display(Name = "LHC Document")]
        public IFormFile? LHCDocument { get; set; }
        [Display(Name = "Shade Rent Agreement")]
        public IFormFile? ShadeRentAgreement { get; set; } 
        [Display(Name = "Business License")]       
        public IFormFile BusinessLicense { get; set; }  
        [Display(Name = "Machine Specification Document")]      
        public IFormFile MachineSpecificationDocument { get; set; }
        [Display(Name = "Machine Operation Manual")]
        public IFormFile? MachineOperationManual { get; set; }
        [Display(Name = "Other Supporting Documents")]
        public IEnumerable<IFormFile>? OtherDocuments { get; set; }
    }
}