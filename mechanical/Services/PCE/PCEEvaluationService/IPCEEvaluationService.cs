using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Entities;
using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationReturnDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationUpdateDto Dto);
        Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id);
        Task<bool> RejectPCEEvaluation(Guid UserId, PCERejectPostDto Dto);
        Task<bool> EvaluatePCEEvaluation(Guid UserId, Guid Id);

        Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id);
        Task<PCEEvaluationReturnDto> GetPCEEvaluationByPCEId(Guid UserId, Guid PCEId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetPCEEvaluationsByPCECaseId(Guid UserId, Guid PCEId);

        Task<PCECasesCountDto> GetDashboardPCECasesCount(Guid UserId);
        Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id);
        Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status, int? Limit = null);

        Task<IEnumerable<ReturnProductionDto>> GetPCEs(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        Task<int> GetPCEsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
       
        Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid PCEId);
        Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId);
        Task<CreateUser> GetUser(Guid UserId);

        Task<PCEEvaluationReturnDto> RemarkReleasePCEEvaluation(Guid UserId, Guid Id, String Remark, Guid EvaluatorId);


        // Task<IEnumerable<ReturnProductionDto>> GetReturnedPCEs(Guid UserId);
        // Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId); 
        // Task<PCEsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null);
        // Task<int> GetPCEsCount(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
    }
}