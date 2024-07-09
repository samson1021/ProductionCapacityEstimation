namespace mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto
{
    public class ProductionCapcityCorrectionPostDto1
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductionCapacityId { get; set; }
        public Guid EquipmentId { get; set; }
        public string Comment { get; set; }
        public string CommentedAttribute { get; set; }
    }
}
