namespace Signature_management.Model.Entities
{
    public class Acknowledgement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LetterRef { get; set; } = string.Empty;
        public string NameOfBeneficiary { get; set; } = string.Empty;
        public string NameOfCRM { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string EmployeePhoneNumber { get; set; } = string.Empty;
        public string EmployeeLocation { get; set; } = string.Empty;
        public string AcknowledgementLetterBase64Data { get; set; }= string.Empty;
        public string Createdby { get; set; }= string.Empty;
        public DateTime CreatedDate { get; set; }

    }
}
