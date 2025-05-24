namespace mechanical.Models.Dto.UploadFileDto
{
    public class ReturnPCEReportFileDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid CollateralId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
