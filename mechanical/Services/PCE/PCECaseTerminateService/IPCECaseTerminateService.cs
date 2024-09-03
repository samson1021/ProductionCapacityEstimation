using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Models.PCE.Dto.PCECaseTerminateDto;

namespace mechanical.Services.PCE.PCECaseTerminateService
{
    public interface IPCECaseTerminateService
    {
        Task<PCECaseTerminateReturnDto> CreateCaseTerminate(Guid userId, PCECaseTerminatePostDto pcecaseTerminatePostDto);
        Task<PCECaseTerminateReturnDto> UpdateCaseTerminate(Guid userId, Guid id, PCECaseTerminatePostDto pcecaseTerminatePostDto);
        Task<PCECaseTerminateReturnDto> ApproveCaseTerminate(Guid id);
        Task<IEnumerable<PCECaseTerminateReturnDto>> GetCaseTerminates(Guid pcecaseId);
    }
}
