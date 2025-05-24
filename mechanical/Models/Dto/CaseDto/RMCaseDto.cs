namespace mechanical.Models.Dto.CaseDto
{
    public class RMCaseDto
    {
        public DateTime CreationAt { get; set; }
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }        
        public required string Center { get; set; }
        public required string Segment { get; set; }
        public int NoOfCollateral { get; set; } = 0;
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
    }
}
