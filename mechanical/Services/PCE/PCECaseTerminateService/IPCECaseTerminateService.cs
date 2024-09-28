using mechanical.Models.PCE.Dto.PCECaseTerminateDto;

namespace mechanical.Services.PCE.PCECaseTerminateService
{
    public interface IPCECaseTerminateService
    {
        Task<PCECaseTerminateReturnDto> CreateCaseTerminate(Guid UserId, PCECaseTerminatePostDto PCECaseTerminatePostDto);
        Task<PCECaseTerminateReturnDto> UpdateCaseTerminate(Guid UserId, Guid Id, PCECaseTerminatePostDto PCECaseTerminatePostDto);
        Task<PCECaseTerminateReturnDto> ApproveCaseTerminate(Guid Id);
        Task<PCECaseTerminateReturnDto> GetCaseTerminate(Guid Id);
        Task<IEnumerable<PCECaseTerminateReturnDto>> GetCaseTerminates(Guid PCECaseId);
        Task<IEnumerable<PCECaseTerminateDto>> GetPCECaseTerminates(Guid UserId);
    }
}
