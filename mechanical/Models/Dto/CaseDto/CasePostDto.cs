namespace mechanical.Models.Dto.CaseDto
{
    public class CasePostDto
    {
        public required string CaseNo { get; set; }
        public required string Segment { get; set; }
       
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public string? CustomerEmail { get; set; } = string.Empty;
        public IFormFile? BussinessLicence { get; set; }
    }
}
