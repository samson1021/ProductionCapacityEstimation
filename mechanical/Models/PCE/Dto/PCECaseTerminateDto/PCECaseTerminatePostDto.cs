namespace mechanical.Models.PCE.Dto.PCECaseTerminateDto
{
    public class PCECaseTerminatePostDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public Guid PCEId { get; set; }
    }
}
