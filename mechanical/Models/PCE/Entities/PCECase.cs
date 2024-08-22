using mechanical.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Entities
{
    public class PCECase
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public string ApplicantName { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime MakerAssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }

        public required Guid RMUserId { get; set; }
        public virtual District? District { get; set; }
        public virtual CreateUser? RMUser { get; set; }
        public virtual UploadFile? BussinessLicence { get; set; }
        //public virtual ICollection<PlantCapacityEstimation>? PCECollaterals { get; set; }
        public virtual ICollection<ProductionCapacity>? ProductionCapacities { get; set; }
    }
}
