using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Dto.Correction;

using mechanical.Models.PCE.Dto.PCEEvaluationDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public interface IPCEEvaluationService
    {
        Task<PCEEvaluationPostDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationPostDto> EditPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetAllPCEEvaluations(Guid UserId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetNewPCEEvaluations(Guid UserId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetRejectedPCEEvaluations(Guid UserId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetTerminatedPCEEvaluations(Guid UserId);
        Task<IEnumerable<PCEEvaluationReturnDto>> GetPendingPCEEvaluations(Guid UserId);
               
        Task<PCEEvaluationReturnDto> CheckPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto);
        Task<PCEEvaluationReturnDto> GetPCEEvaluationByPCEId(Guid UserId, Guid PCEId);
        Task<PCEEvaluationReturnDto> GetEvaluatedPCEEvaluation(Guid UserId, Guid Id);
        Task<PCEEvaluationPostDto> GetReturnedPCEEvaluation(Guid UserId, Guid Id);
        
        Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId);
        Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id);
        Task<bool> SendToRM(Guid UserId, Guid PCEId);
        Task SendForApproval(Guid UserId, IEnumerable<Guid> PCEIds, Guid CenterId);
        Task RejectPCEEvaluation(Guid UserId, Guid Id, string RejectionReason);
        Task<string> GetPCEEvaluationSummary(Guid UserId, Guid PCEId);        
        Task RemarkRelease(Guid UserId, Guid Id, Guid PCEId, String Remark, Guid EvaluatorID);

    }
}
