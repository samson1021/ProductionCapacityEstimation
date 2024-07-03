namespace mechanical.Models.Dto.UploadFileDto
{
    public class CreateFileDto
    {
        public required IFormFile File { get; set; }
        public string Catagory { get; set; } = string.Empty;
        public Guid CaseId { get; set; }
        public Guid? CollateralId { get; set; }
    }
}
