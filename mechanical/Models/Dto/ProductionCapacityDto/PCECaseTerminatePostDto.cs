namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class PCECaseTerminatePostDto
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public Guid PCEId { get; set; }
    }
}
