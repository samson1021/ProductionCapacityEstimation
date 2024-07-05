using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Dto.SignatureOfEmployeInfo;
using mechanical.Models.Entities;

namespace mechanical.Services.SignatureService
{
    public interface ISignatureService
    {
        Task<Signatures> CreateSignature(Guid userId, SignatureDto signatureDto);
        Task<IEnumerable<ReturnSignatureDto>> Getsignature(string Id);
        Task<EmployeeInfoes>GetEmployeeName(string emp_ID);
    }
}

