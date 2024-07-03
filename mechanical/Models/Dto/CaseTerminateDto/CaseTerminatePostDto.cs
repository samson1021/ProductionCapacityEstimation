namespace mechanical.Models.Dto.CaseTerminateDto
{
    public class CaseTerminatePostDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public Guid CaseId { get; set; }
       // public Guid ProductionCaseId { get; set; }= Guid.Empty;

    }
}
