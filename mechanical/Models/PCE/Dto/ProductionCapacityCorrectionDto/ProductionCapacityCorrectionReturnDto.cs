namespace mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto
{
    public class ProductionCapacityCorrectionReturnDto
    {
        public Guid PCECaseId { get; set; }
        public Guid ProductionCapacityId { get; set; }
        public Guid EquipmentId { get; set; }
        public Guid CommentedByUserId { get; set; }
        public string? Comment { get; set; }
        public string? CommentedAttribute { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}