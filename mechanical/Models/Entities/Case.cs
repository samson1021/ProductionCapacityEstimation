using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mechanical.Models.Entities
{
    public class Case
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }
        [ForeignKey("BussinessLicence")]
        public Guid? BussinessLicenceId { get; set; }
        public bool SharedTask { get; set; }

        public required string Status { get; set; }
        public required string Segement { get; set; }
        public required Guid CaseOriginatorId { get; set; }
        public DateTime CreationAt { get; set; }
        public virtual District? District { get; set; }
        public virtual CreateUser? CaseOriginator { get; set; }
        public virtual UploadFile? BussinessLicence { get; set; }
        public virtual ICollection<Collateral>? Collaterals { get; set; }
    }
}