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
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationReturnDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationReturnDto Dto);
        Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id);
        Task<bool> RejectPCEEvaluation(Guid UserId, PCERejectPostDto Dto);
        Task<bool> EvaluatePCEEvaluation(Guid UserId, Guid Id);
        // Task<bool> ReevaluatePCEEvaluation(Guid UserId, Guid Id);    
        Task<bool> ReworkPCEEvaluation(Guid UserId, Guid Id);     
        // Task<bool> ReworkPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);     
        Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id);
        Task<PCEEvaluationReturnDto> GetPCEEvaluationsByPCEId(Guid UserId, Guid PCEId);
        
        Task<MyPCECaseCountDto> GetDashboardPCECaseCount(Guid userId);
        Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id);
        Task<IEnumerable<PCECaseReturntDto>> GetPCECasesWithStatus(Guid UserId, string status); 
        Task<IEnumerable<PCECaseReturntDto>> GetTotalPCECases(Guid UserId);  

        Task<IEnumerable<ReturnProductionDto>> GetProductionCapacities(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetProductionCapacitiesWithStatus(Guid PCECaseId, string status);
        Task<IEnumerable<ReturnProductionDto>> GetProductionCapacitiesWithStatusAndRole(Guid PCECaseId, string status, string role);

        // Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId);
        // // Task<IEnumerable<ReturnProductionDto>> RejectedPCEs(Guid UserId);
        // // Task<IEnumerable<ReturnProductionDto>> ResubmittedPCEs(Guid UserId);
    }
}
