namespace Signature_management.Model.Dto.AcknowledgementDto
{
    public class AcknowledgementReturnDto
    {
        public string EmployeeID { get; set; } = string.Empty;
        public string AcknowledgementLetterBase64String { get; set; } = string.Empty;
        public string ErrorMessage { get; set; }= string.Empty; 
    }
}
