using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using mechanical.Models.PCE.Dto.PCEDto;

namespace mechanical.Services.PCE.PCEService
{
    public interface IPCEService
    {
        Task<PCEPostDto> CreatePCE(Guid UserId, Guid PCEcaseId, PCEPostDto dto);
        Task<PCEPostDto> EditPCE(Guid UserId, Guid id, PCEPostDto dto);
        Task<PCEReturnDto> GetPCE(Guid UserId, Guid id);
        Task<IEnumerable<PCEReturnDto>> GetAllPCEs(Guid UserId);
        Task<IEnumerable<PCEReturnDto>> GetNewPCEs(Guid UserId);
        Task<IEnumerable<PCEReturnDto>> GetRejectedPCEs(Guid UserId);
        Task<IEnumerable<PCEReturnDto>> GetTerminatedPCEs(Guid UserId);
        Task<IEnumerable<PCEReturnDto>> GetPendingPCEs(Guid UserId);
        Task SendForApproval(string selectedPCEIds, string CenterId);
        Task RejectPCE(Guid id, string rejectionReason);
        Task<PCEScheduleDto> CreateSchedule(Guid UserId, PCEScheduleDto scheduleDto);
        Task<int> GetDashboardPCECount(Guid UserId);
        Task<int> GetMyDashboardPCECount(Guid UserId);
        Task<bool> DeleteSupportingEvidence(Guid Id);
        Task<bool> DeleteProcessFlowDiagram(Guid Id);
        Task<bool> DeletePCE(Guid UserId, Guid id);
        // Task<bool> UploadSupportingEvidence(Guid UserId, IFormFile file, Guid PCEcaseId);
        // Task<bool> UploadProcessFlowDiagram(Guid UserId, IFormFile file, Guid PCEcaseId);
    }
}
