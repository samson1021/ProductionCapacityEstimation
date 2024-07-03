namespace mechanical.Models.Dto.ProductionCapcityCorrectionDto
{
    public class ProductionCapcityCorrectionPostDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductionCapacityId { get; set; }
        public Guid EquipmentId { get; set; }
        public string Comment { get; set; }
        public string CommentedAttribute { get; set; }
    }
}
