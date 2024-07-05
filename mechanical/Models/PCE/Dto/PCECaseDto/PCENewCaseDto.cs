using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCENewCaseDto
    {

        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerUserId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }
   public DateTime CreationDate { get; set; }

        public string CurrentStatus { get; set; } = string.Empty;
     

    }
}
