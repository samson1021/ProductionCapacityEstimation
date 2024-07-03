namespace mechanical.Models.Dto.SignatureDto
{
    public class SignatureDto
    {
        public required IFormFile File { get; set; }
        public string Emp_Id { get; set; } = string.Empty;
    }
}
