using mechanical.Models.Enum.ProductionCapcityEstimation;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class ProductionPostDto
    {
        public required Guid ProductionCaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        [Display(Name = "Manufacturing Main-Sector")]
        public ManufacturingSector Category { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public string Type { get; set; }
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        public string ProductionBussinessLicence { get; set; }

        [Range(1900, 2024)]
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ProductionType { get; set; }
        public string? SerialNo { get; set; }

        public MachineryInstalledPlace MachineryInstalledPlace { get; set; }
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
        public string ProductDescription { get; set; }

        public IFormFile? UploadLHC { get; set; }
        public IFormFile? ploadshaderentagreement { get; set; }
        public IFormFile? Uploadbusinesslicense { get; set; }
        public IFormFile? Machinespecificationdocumen { get; set; }
        public IFormFile? Machineoperationmanual { get; set; }
        public IEnumerable<IFormFile>? OtherDocument { get; set; }

    }
}
