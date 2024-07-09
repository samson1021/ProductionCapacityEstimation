using mechanical.Models.Dto.CaseScheduleDto;

namespace mechanical.Services.CaseScheduleService
{
    public interface ICaseScheduleService
    {
        Task<CaseScheduleReturnDto> CreateCaseSchedule(Guid userId, CaseSchedulePostDto caseCommentPostDto);
        Task<CaseScheduleReturnDto> UpdateCaseSchedule(Guid userId,Guid id, CaseSchedulePostDto caseCommentPostDto);
        Task<CaseScheduleReturnDto> ApproveCaseSchedule(Guid id);
        Task<IEnumerable<CaseScheduleReturnDto>> GetCaseSchedules(Guid caseId);
    }
}
