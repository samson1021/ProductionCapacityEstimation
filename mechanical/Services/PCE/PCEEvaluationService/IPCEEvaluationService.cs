using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationPostDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> ReturnPCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationPostDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> PendPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> EvaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> ReevaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> CompletePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id);
        Task<bool> SendToRM(Guid UserId, Guid Id);    
        Task<bool> SendToMO(Guid UserId, Guid Id);    
        Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id);
        Task<PCEEvaluationReturnDto> GetPCEEvaluationsByPCEId(Guid UserId, Guid PCEId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetAllPCEEvaluations(Guid UserId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetPCEEvaluationsWithStatus(Guid UserId, Status Status);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetNewPCEEvaluations(Guid UserId);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetPendingPCEEvaluations(Guid UserId);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetEvaluatedPCEEvaluations(Guid UserId);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetReturnedPCEEvaluations(Guid UserId);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetReevaluatePCEEvaluations(Guid UserId);
        // Task<IEnumerable<PCEEvaluationReturnDto>> GetCompletedPCEEvaluations(Guid UserId);

        Task<PCEEvaluationReturnDto> ReworkPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> ApprovePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task CompletePCEEvaluations(Guid UserId, IEnumerable<Guid> SelectedPCEIds, Guid CenterId);
        
        Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId);
        Task<IEnumerable<int>> GetDashboardPCEEvaluationCount(Guid UserId);    

    }
}
