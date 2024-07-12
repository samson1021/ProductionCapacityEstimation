using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.RMDashboardDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationReturnDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        // Task<PCEEvaluationReturnDto> ReturnPCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> PendPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> EvaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> ReevaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> CompletePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id);
        Task<bool> ReturnPCEEvaluation(Guid UserId, PCERejectPostDto Dto);
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
        Task<MyPCECaseCountDto> GetMyDashboardPCECaseCount(Guid userId);

        Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid id);
        Task<PCECaseReturntDto> GetPCECaseDetail(Guid UserId, Guid id);
        Task<IEnumerable<PCECaseReturntDto>> GetNewPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturntDto>> GetPendingPCECases(Guid UserId); 
        Task<IEnumerable<PCECaseReturntDto>> GetCompletedPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturntDto>> GetReturnedPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturntDto>> GetResubmitedPCECases(Guid UserId); 
        Task<IEnumerable<PCECaseReturntDto>> GetTotalPCECases(Guid UserId);  

        Task<ReturnProductionDto> MyReturnedPCE(Guid UserId, Guid id);
        Task<ReturnProductionDto> MyResubmitedPCE(Guid UserId, Guid id);
        // Task<IEnumerable<ReturnProductionDto>> MyReturnedPCEs(Guid UserId);
        // Task<IEnumerable<ReturnProductionDto>> MyResubmitedPCEs(Guid UserId);

        
    }
}