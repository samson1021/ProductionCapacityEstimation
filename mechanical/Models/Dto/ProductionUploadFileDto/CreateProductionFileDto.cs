namespace mechanical.Models.Dto.ProductionUploadFileDto
{
    public class CreateProductionFileDto
    {
        public required IFormFile File { get; set; }
        public string Catagory { get; set; } = string.Empty;
        public Guid ProductionCaseId { get; set; }     
        public Guid? ProductionCapacityId { get; set; }
    }
}
