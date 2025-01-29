using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public interface IPCECaseScheduleService
    {
        Task<PCECaseScheduleReturnDto> CreateSchedule(Guid UserId, PCECaseSchedulePostDto Dto); 
        Task<PCECaseScheduleReturnDto> UpdateSchedule(Guid UserId, PCECaseSchedulePostDto Dto);
        Task<PCECaseScheduleReturnDto> ProposeSchedule(Guid UserId, PCECaseSchedulePostDto Dto);
        Task<PCECaseScheduleReturnDto> CreateReschedule(Guid UserId, PCECaseSchedulePostDto Dto);
        Task<PCECaseScheduleReturnDto> ApproveSchedule(Guid UserId, Guid Id);

        Task<PCECaseScheduleReturnDto> GetSchedule(Guid Id);
        Task<PCECaseScheduleReturnDto> GetLatestSchedule(Guid PCECaseId);
        Task<IEnumerable<PCECaseScheduleReturnDto>> GetSchedules(Guid PCECaseId);
    }
}