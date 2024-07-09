using mechanical.Models.Dto.CaseTerminateDto;

namespace mechanical.Services.CaseTerminateService
{
    public interface ICaseTerminateService
    {
        Task<CaseTerminateReturnDto> CreateCaseTerminate(Guid userId, CaseTerminatePostDto caseTerminatePostDto);
        Task<CaseTerminateReturnDto> UpdateCaseTerminate(Guid userId,Guid id, CaseTerminatePostDto caseTerminatePostDto);
        Task<CaseTerminateReturnDto> ApproveCaseTerminate(Guid id);
        Task<IEnumerable<CaseTerminateReturnDto>> GetCaseTerminates(Guid caseId);
    }
}
