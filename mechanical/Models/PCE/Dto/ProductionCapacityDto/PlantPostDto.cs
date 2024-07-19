using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class PlantPostDto
    {
        public required Guid PCECaseId { get; set; }
        [Display(Name = "Collateral type")]

        public required string ProductionType { get; set; }
        [Display(Name = "Propery Owner")]

        public required string PropertyOwner { get; set; }
        public required string Role { get; set; } //i dont understand what it mean


        [Display(Name = "Trade License Number")]
        public required string TradeLicenseNumber { get; set; }
        [Display(Name = "Land Holding Certificate Number")]
        public required string LHCNumber { get; set; }

        [Display(Name = "Owner Name of LHC")]
        public required string OwnerNameLHC { get; set; }
        [Display(Name = "Name of the Plant")]
        public required string PlantName { get; set; }
        public string? OtherPlantName { get; set; }

        // [Range(1900,DateTime())]
        [Display(Name = "Year of manufacturing")]
        public int YearOfManifacturing { get; set; }
        [Display(Name = "Purose of the Collateral")]
        public required string PurposeOfCollateral { get; set; }
        public string? ProductDescription { get; set; }
        [Display(Name = "Obsolescence status")]
        public required string ObsolescenceStatus { get; set; }
        [Display(Name = "Plant Depreciation Rate")]
        public required string PlantDepreciationRate { get; set; }
        [Display(Name = "Owner of the Machinery")]
        public required string OwnerOfMachinery { get; set; }
        [Display(Name = "Country of Orgin")]
        public string? CountryOfOrgin { get; set; }
        [Display(Name = "Place of Inspection")]
        public string? PlaceOfInspection { get; set; }


        //address of the plant
        public required string Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }

        [Display(Name = "Date of Inspection")]
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
        [Display(Name = "Supportive document")]
        public IFormFile? CBEPartialFinancing { get; set; }




    }
}
