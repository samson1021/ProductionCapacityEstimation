using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Dto.CollateralDto
{
    public class CollateralPostDto
    {
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public MechanicalCollateralCategory Category { get; set; }
        public MechanicalCollateralType Type { get; set; }
        [RegularExpression(@"^\d{2}-[A-Z]{2}-[A-Z]?\d{5}$", ErrorMessage = "Please enter a valid Plate Number")]
        public string? PlateNo { get; set; }
        public string? ChassisNo { get; set; }
        public string? EngineMotorNo { get; set; }
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? SerialNo { get; set; }
        public required string Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? CollateralType { get; set; }
        //industry collaterals attribute 
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        //Civil
        public string? BlockNo { get; set; }
        public string? FloorNo { get; set; }
        public string? BuildingStatus { get; set; }
        public string? HouseArea { get; set; }
        public string? Ownership { get; set; }
        public string? LHCNo { get; set; }
       
        public required IFormFile TitleDeed { get; set; }
        public required IFormFile CommercialInvoice { get; set; }
        public required IFormFile CustomDeclaration { get; set; }
        public IFormFile? PackingList { get; set; }
        public IFormFile? SalesDocument { get; set; }
        public IEnumerable<IFormFile>? OtherDocument { get; set; }

    }
}
