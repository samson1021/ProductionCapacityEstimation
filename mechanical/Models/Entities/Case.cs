using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseOriginatorId))]
    public class Case
    {
        [Key]
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }
        public Guid? BussinessLicenceId { get; set; }
        public bool SharedTask { get; set; }

        public required string Status { get; set; }
        public required string Segment { get; set; }
        public required Guid CaseOriginatorId { get; set; }
        public DateTime CreationAt { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District? District { get; set; }
        [ForeignKey("CaseOriginatorId")]
        public virtual User? CaseOriginator { get; set; }
        [ForeignKey("BussinessLicenceId")]
        public virtual UploadFile? BussinessLicence { get; set; }
        public virtual ICollection<Collateral>? Collaterals { get; set; }
    }
}