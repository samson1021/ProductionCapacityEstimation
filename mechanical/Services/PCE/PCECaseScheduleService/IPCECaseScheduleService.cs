using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public interface IPCECaseScheduleService
    {
        Task<PCECaseSchedulePostDto> CreatePCECaseSchedule(Guid UserId, PCECaseSchedulePostDto dto);
        Task<PCECaseSchedulePostDto> EditPCECaseSchedule(Guid UserId, Guid id, PCECaseSchedulePostDto dto);
        Task<PCECaseScheduleReturnDto> GetPCECaseSchedule(Guid UserId, Guid id);        
        Task<bool> DeletePCECaseSchedule(Guid UserId, Guid id);
        // Task<PCECaseScheduleReturnDto> CheckPCECaseSchedule(Guid userId, Guid Id, PCECaseSchedulePostDto dto);
        // Task<PCECaseScheduleReturnDto> GetPCECaseScheduleByPCEId(Guid UserId, Guid PCEId);
    }
}
