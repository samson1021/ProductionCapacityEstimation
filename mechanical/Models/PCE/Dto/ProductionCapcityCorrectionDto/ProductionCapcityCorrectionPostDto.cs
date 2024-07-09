namespace mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto
{
    public class ProductionCapcityCorrectionPostDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PCECaseId { get; set; }
        public Guid EquipmentId { get; set; }
        public string Comment { get; set; }
        public string CommentedAttribute { get; set; }
    }
}
