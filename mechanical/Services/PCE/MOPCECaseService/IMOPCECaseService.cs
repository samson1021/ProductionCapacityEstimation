using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using mechanical.Models.Entities;
using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;

namespace mechanical.Services.PCE.MOPCECaseService
{
    public interface IMOPCECaseService
    {
        Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id);
        Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null);
        Task<IEnumerable<ReturnProductionDto>> GetPCEs(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        Task<int> GetPCEsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        Task<PCECasesCountDto> GetDashboardPCECasesCount(Guid UserId);
        Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid PCEId);
        Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId);
        Task<IEnumerable<PCENewCaseDto>> GetRemarkedPCECases(Guid UserId); 
        Task<CreateUser> GetUser(Guid UserId);

        // Task<IEnumerable<ReturnProductionDto>> GetReturnedPCEs(Guid UserId);
        // Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId); 
        // Task<PCEsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null);
        // Task<int> GetPCEsCount(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
    }
}