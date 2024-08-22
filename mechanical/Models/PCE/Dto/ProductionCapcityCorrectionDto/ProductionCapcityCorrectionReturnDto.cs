namespace mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto
{
    public class ProductionCapcityCorrectionReturnDto
    {
        public Guid ProductionCaseId { get; set; }
        public Guid ProductionCapacityId { get; set; }
        public Guid EquipmentId { get; set; }
        public Guid CommentedByUserId { get; set; }
        public string? Comment { get; set; }
        public string? CommentedAttribute { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}