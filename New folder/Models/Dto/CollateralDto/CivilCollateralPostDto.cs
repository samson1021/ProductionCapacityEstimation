using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Dto.CollateralDto
{
    public class CivilCollateralPostDto
    {
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? CollateralType { get; set; }
        public string? Region { get; set; }
        public string? City  { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Woreda { get; set; }
        public string? Kebele { get; set; }

        public string? PurposeOfTheLand { get; set; }
        public string? PlotOfLand { get; set; }
        public string? LHCNo { get; set; }
        public string? BlockNo { get; set; }
        public string? FloorNo { get; set; }
        public string? BuildingType { get; set; }
        public string? BuildingStatus { get; set; }
        public string? HouseArea { get; set; }
        public string? HouseNo { get; set; }
        public string? Ownership { get; set; }

        public IFormFile? UploadLHC { get; set; }
        public IFormFile? UploadSitePlan { get; set; }
        public IFormFile? LeaseAgreement { get; set; }
        public IFormFile? CurrentLeasePaymentReceipt { get; set; }
        public IFormFile? CurrentLandRevenueTaxPaymentReceipt { get; set; }
        public IFormFile? ApplicantsExactHoldingConfirmedByAssociation { get; set; }
        public IFormFile? LetterFromTheCooperativeAssociationWithMinutes { get; set; }
        public IFormFile? ValidConstructionPermit { get; set; }
        public IFormFile? BillOfQuantity { get; set; }
        public IFormFile? RenewedConsultantProfessionalLicense { get; set; }
        public IEnumerable<IFormFile>? OtherDocument { get; set; }

    }
}
