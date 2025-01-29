namespace mechanical.Models.Dto.UploadFileDto
{
    public class ReturnFileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Catagory { get; set; } = string.Empty;
    }
}
