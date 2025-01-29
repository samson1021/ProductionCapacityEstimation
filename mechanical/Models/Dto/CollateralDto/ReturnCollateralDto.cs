using mechanical.Models.Enum;
namespace mechanical.Models.Dto.CollateralDto
{
    public class ReturnCollateralDto
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? Category { get; set; } 
        public string? Type { get; set; }
        public  string? PlateNo { get; set; }
        public  string? ChassisNo { get; set; }
        public string? EngineMotorNo { get; set; }
        public int? ManufactureYear { get; set; }
        public string? InvoiceNo { get; set; }
        public string? SerialNo { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? CurrentStatus { get; set; }
        public string? CurrentStage { get; set; }
        //industry collaterals attribute 
        public string? MachineName { get; set; }
        public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        public string? CollateralType { get; set; }
    }
}
