using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Dto.CollateralDto
{
    public class AgricultureCollateralPostDto
    {
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? CollateralType { get; set; }
        public string? TypeOfFarming { get; set; }
        public string? PurposeOfTheLand { get; set; }
        public string? PlotOfLand { get; set; }
        public string? LHCNo { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Woreda { get; set; }
        public string? Kebele { get; set; }
        public IFormFile? UploadLHC { get; set; }
        public IFormFile? UploadSitePlan { get; set; }
        public IFormFile? LeaseAgreement { get; set; }
        public IFormFile? CurrentLeasePaymentReceipt { get; set; }
        public IEnumerable<IFormFile>? OtherDocument { get; set; }

    }
}
