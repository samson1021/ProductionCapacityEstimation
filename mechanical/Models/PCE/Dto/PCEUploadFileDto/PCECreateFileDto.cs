namespace mechanical.Models.PCE.Dto.PCEUploadFileDto
{
    public class PCECreateFileDto
    {
        public required IFormFile File { get; set; }
        public string Catagory { get; set; } = string.Empty;
        public Guid CaseId { get; set; }
        public Guid? PlantCapacityEstimationId { get; set; }
    }
}
