namespace mechanical.Models.Entities
{
    public class CaseAssignment
    {
        public Guid Id { get; set; }
        public Guid? CollateralId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;


        public virtual Collateral? Collateral { get; set; }
        public virtual User? User { get; set; }
    }
}
