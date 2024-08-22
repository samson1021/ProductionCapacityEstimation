using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECaseReturntDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public string CustomerUserId { get; set; } = string.Empty; 
        public string CustomerEmail { get; set; } = string.Empty;
        public  string? District { get; set; }
        public  string? Type { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime MakerAssignmentDate { get; set; }
        public int TotalNoOfCollateral { get; set; } = 0;
        public virtual UploadFile? BussinessLicence { get; set; }




    }
}
