using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECaseReturnDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string Segment { get; set; }
        public required string ApplicantName { get; set; }
        // public required string RequestingUnit { get; set; }
        public  string? Type { get; set; } 
        public string CustomerId { get; set; } = string.Empty; 
        public string CustomerEmail { get; set; } = string.Empty;
        
        public Guid DistrictId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string Status { get; set; } = string.Empty; 
        public string? District { get; set; }
        public int NoOfProductions { get; set; } = 0;
        public int TotalNoOfProductions { get; set; } = 0;

        public virtual UploadFile? BusinessLicense { get; set; }
    }
}
