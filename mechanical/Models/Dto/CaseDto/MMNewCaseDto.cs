namespace mechanical.Models.Dto.CaseDto
{
    public class MMNewCaseDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public required string Segment { get; set; }
        public DateTime CreationAt { get; set; }
        public required Guid RMUserId { get; set; }
        public int NoOfCollateral { get; set; } = 0;
    }
}
