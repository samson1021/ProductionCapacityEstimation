namespace mechanical.Models.Dto.CaseDto
{
    public class CONewCaseDto
    {
        public Guid Id { get; set; }
        public DateTime MakerSendDate { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string Segment { get; set; }
        public required string CustomerId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime CreationAt { get; set; }
        public required Guid RMUserId { get; set; }
        public int NoOfCollateral { get; set; } = 0;
    }
}
