using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class PlantCapacityEstimationPostDto
    {
        public required Guid PCECaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? PlantName { get; set; }
        public string? Purpose { get; set; }
        public string Type { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? OwnerOfPlant { get; set; }
        public string? ProductionBussinessLicence { get; set; }
        public int? ManufactureYear { get; set; }
        public string? ProductionType { get; set; }
        public string? LHCNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ObsolescenceStatus { get; set; }
        public string? PlantDepreciationRate { get; set; }
        public DateTime? DateOfInspection { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? CurrentStage { get; set; }
        public string? CurrentStatus { get; set; }
        public Guid CreatedById { get; set; }


        public required IFormFile CommercialInvoice { get; set; }
        public required IFormFile customDeclaration { get; set; }
        public required IFormFile LHC { get; set; }
        public required IFormFile BussinessLicence { get; set; }
        public IEnumerable<IFormFile>? OtherDocument { get; set; }


        //commercial Invoice
        //    customs declaration 
        //    LHC DOC
        //    Bussiness License
        //    any other document

    }
}
