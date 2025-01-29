namespace mechanical.Models.Dto.CaseAssignmentDto
{
    public class CaseAssignmentDto
    {
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
    }
}
