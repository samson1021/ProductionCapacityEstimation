namespace mechanical.Models.Dto.ProductionCaseDto
{
    public class ProductionCasePostDto
    {
        public required string CaseNo { get; set; }
        public required string Segement { get; set; }

        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public IFormFile? ProductionBussinessLicence { get; set; }
    }
}
