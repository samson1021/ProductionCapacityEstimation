using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    public class PCECase
    {
        [Key]
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public required string ApplicantName { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; } = string.Empty;

        public required string Segment { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Guid DistrictId { get; set; }
        public Guid? BusinessLicenseId { get; set; }
        public required Guid PCECaseOriginatorId { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District? District { get; set; }

        [ForeignKey("PCECaseOriginatorId")]
        public virtual User? PCECaseOriginator { get; set; }

        [ForeignKey("BusinessLicenseId")]
        public virtual UploadFile? BusinessLicense { get; set; }
        public virtual ICollection<ProductionCapacity>? ProductionCapacities { get; set; }
    }
}
