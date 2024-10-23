using mechanical.Models.PCE.Enum.ProductionCapacity;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class ProductionPostDto
    {
        public required Guid PCECaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        [Display(Name = "Manufacturing Main-Sector")]
        public ManufacturingSector Category { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public string Type { get; set; }
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        public string BusinessLicenseNumber { get; set; }
        // public string TradeLicenseNumber { get; set; }

        [Display(Name = "Country of Orgin")]
        public string? CountryOfOrgin { get; set; }

        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ProductionType { get; set; }
        public string? SerialNo { get; set; }

        public string? MachineryInstalledPlace { get; set; }
        public string? LHCNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? Industrialpark { get; set; }

        public string? Region { get; set; }
        public string? Zone { get; set; }
        public string? City { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
         public string? HouseNo { get; set; }
        public string? ProductDescription { get; set; }

        public IFormFile? LHCDocument { get; set; }
        public IFormFile? ShadeRentAgreement { get; set; }        
        public IFormFile? BusinessLicense { get; set; }        
        public IFormFile? MachineSpecificationDocument { get; set; }
        public IFormFile? MachineOperationManual { get; set; }
        public IEnumerable<IFormFile>? OtherDocuments { get; set; }
    }
}