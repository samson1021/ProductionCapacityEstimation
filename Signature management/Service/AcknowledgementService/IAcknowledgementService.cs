using Signature_management.Model.Dto.AcknowledgementDto;

namespace Signature_management.Service.AcknowledgementService
{
    public interface IAcknowledgementService
    {
        AcknowledgementReturnDto CreateAcknowledgementLetter(AcknowledgementPostDto acknowledgementPost);
        AcknowledgementReturnDto getAcknowledgementLetter(string EmployeeID);
    }
}
