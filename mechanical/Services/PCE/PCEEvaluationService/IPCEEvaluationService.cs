using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationReturnDto> CreateValuation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> UpdateValuation(Guid UserId, Guid Id, PCEEvaluationUpdateDto Dto);
        Task<bool> DeleteValuation(Guid UserId, Guid Id);
        Task<bool> ReturnValuation(Guid UserId, ReturnedProductionPostDto Dto);
        Task<bool> CompleteValuation(Guid UserId, Guid Id);

        Task<bool> HandleRemark(Guid UserId, Guid PCEId, String RemarkType, CreateFileDto FileDto, Guid EvaluatorId);
        Task<PCEEvaluationReturnDto> ReleaseRemark(Guid UserId, Guid Id, String Remark, Guid EvaluatorId);
        
        Task<PCEEvaluationReturnDto> GetValuation(Guid UserId, Guid Id);
        Task<PCEEvaluationReturnDto> GetValuationByPCEId(Guid UserId, Guid PCEId);
        Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetValuationsByPCECaseId(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetValuationsSummaryByPCECaseId(Guid UserId, Guid PCECaseId);
        //HO
        Task<PCEEvaluationReturnDto> GetHOValuation(Guid Id);
        Task<PCEEvaluationReturnDto> GetHOValuationByPCEId(Guid PCEId);
        Task<PCEValuationHistoryDto> GetHOValuationHistory(Guid PCEId);
    }
}