namespace mechanical.Models.Dto.CaseDto
{
    public class MONewCaseDto
    {
        public Guid Id { get; set; }
        public DateTime AssignmentDate { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public required string Segement { get; set; }
        public DateTime CreationAt { get; set; }
        public required Guid RMUserId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public int NoOfCollateral { get; set; } = 0;
    }
}
