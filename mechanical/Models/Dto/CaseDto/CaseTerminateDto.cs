namespace mechanical.Models.Dto.CaseDto
{
    public class CaseTerminateDto
    {
        public Guid Id { get; set; }
        public DateTime CreationAt { get; set; }
        public required string CaseNo { get; set; }
        public required string Segment { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public string? CustomerEmail { get; set; } = string.Empty;
        public string District {  get; set; } = string.Empty;
        public required string Status { get; set; }
        public int NoOfCollateral { get; set; } = 0;
        public  string? TerminationReason { get; set; }

    }
}
