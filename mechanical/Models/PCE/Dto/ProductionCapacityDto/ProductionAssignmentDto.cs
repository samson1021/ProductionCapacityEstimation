namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class ProductionAssignmentDto
    {
        public required Guid PCECaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public Guid ProductionCaseAssignmentId { get; set; }
        public Guid? ProductionCapacityId { get; set; }
        public required String User { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
