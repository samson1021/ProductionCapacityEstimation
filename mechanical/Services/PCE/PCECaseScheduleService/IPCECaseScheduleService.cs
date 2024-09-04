using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public interface IPCECaseScheduleService
    {
        Task<ProductionCaseScheduleReturnDto> CreateCaseSchedule(Guid userId, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> UpdateCaseSchedule(Guid userId, Guid id, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> ApproveCaseSchedule(Guid id);
        Task<IEnumerable<ProductionCaseScheduleReturnDto>> GetCaseSchedules(Guid caseId);
    }
}
