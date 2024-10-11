namespace mechanical.Models.PCE.Dto.PCECaseTerminateDto
{
    public class PCECaseTerminateDto
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public required string CaseNo { get; set; }       
        public required string ApplicantName { get; set; }
        public required string CustomerUserId { get; set; }
        public string District { get; set; } = string.Empty;
        public required string Status { get; set; }
        public int TotalNoOfProductions { get; set; } = 0;
        public string? TerminationReason { get; set; }
    }
}
