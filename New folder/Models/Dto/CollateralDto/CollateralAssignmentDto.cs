namespace mechanical.Models.Dto.CollateralDto
{
    public class CollateralAssignmentDto
    {
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public Guid CaseAssigmentId { get; set; }
        public Guid? CollateralId { get; set; }
        public required String User { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
