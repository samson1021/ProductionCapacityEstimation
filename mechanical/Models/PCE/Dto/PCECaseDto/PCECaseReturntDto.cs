using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECaseReturntDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime MakerAssignmentDate { get; set; }


       
    }
}
