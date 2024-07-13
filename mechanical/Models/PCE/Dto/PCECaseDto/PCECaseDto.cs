namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECaseDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerUserId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid DistrictId { get; set; }


    }
}
