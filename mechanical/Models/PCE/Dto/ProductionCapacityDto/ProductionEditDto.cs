using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class ProductionEditDto
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
        public string MachineName { get; set; }

        [Display(Name = "Country of Origin")]
        public required string CountryOfOrigin { get; set; }

        [Display(Name = "Business License Number")]
        public string BusinessLicenseNumber { get; set; }
       
        [Display(Name = "Purpose")]
        public string? Purpose { get; set; }

        [Display(Name = "Model Number")]
        public string? ModelNo { get; set; }

        [Display(Name = "Manufacture Year")]
        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }

        [Display(Name = "Invoice Number")]
        public string? InvoiceNo { get; set; }

        [Display(Name = "Serial Number")]
        public string? SerialNo { get; set; }

        [Display(Name = "Machinery Installed Place")]
        public required string MachineryInstalledPlace { get; set; }

        [Display(Name = "LHC Number")]
        public string? LHCNumber { get; set; }

        [Display(Name = "LHC Owner Name")]
        public string? OwnerName { get; set; }

        [Display(Name = "Industrial Park")]
        public string? Industrialpark { get; set; }

        [Display(Name = "Region")]
        public required string Region { get; set; }

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

        [Display(Name = "Remark")]
        public string? Remark { get; set; } = string.Empty;
    }
}
