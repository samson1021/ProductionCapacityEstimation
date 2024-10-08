using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public interface IPCECaseScheduleService
    {
        Task<PCECaseScheduleReturnDto> CreatePCECaseSchedule(Guid UserId, PCECaseSchedulePostDto Dto);
        Task<PCECaseScheduleReturnDto> UpdatePCECaseSchedule(Guid UserId, Guid Id, PCECaseSchedulePostDto Dto);
        Task<PCECaseScheduleReturnDto> ApprovePCECaseSchedule(Guid Id);
        Task<IEnumerable<PCECaseScheduleReturnDto>> GetPCECaseSchedules(Guid PCECaseId);
    }
}


