using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;

namespace Signature_management.Model.Dto.AcknowledgementDto
{
    public class AcknowledgementPostDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();   
        public string LetterRef { get; set; } = string.Empty;
        public string NameOfBeneficiary { get; set; } = string.Empty;
        public string NameOfCRM { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string EmployeePhoneNumber { get; set; } = string.Empty;
        public string EmployeeLocation { get; set; } = string.Empty;


    }
}
