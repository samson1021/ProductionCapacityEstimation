using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using mechanical.Models.Dto.ProductionCapacityDto;

namespace mechanical.Services.ProductionCapacityService
{
    public interface IProductionCapacityEstimationService
    {
        Task<ProductionCapacityEstimationPostDto> CreateProductionCapacityEstimation(Guid UserId, Guid PCEcaseId, ProductionCapacityEstimationPostDto dto);
        Task<ProductionCapacityEstimationPostDto> EditProductionCapacityEstimation(Guid UserId, Guid id, ProductionCapacityEstimationPostDto dto);
        Task<ProductionCapacityEstimationReturnDto> GetProductionCapacityEstimation(Guid UserId, Guid id);
        Task<IEnumerable<ProductionCapacityEstimationReturnDto>> GetAllProductionCapacityEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationReturnDto>> GetNewEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationReturnDto>> GetRejectedEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationReturnDto>> GetTerminatedEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationReturnDto>> GetPendingEstimations(Guid UserId);
        Task SendForApproval(string selectedEstimationIds, string CenterId);
        Task RejectEstimation(Guid id, string rejectionReason);
        Task<ProductionCapacityScheduleDto> CreateSchedule(Guid UserId, ProductionCapacityScheduleDto scheduleDto);
        Task<int> GetDashboardEstimationCount(Guid UserId);
        Task<int> GetMyDashboardEstimationCount(Guid UserId);
        Task<bool> DeleteSupportingEvidence(Guid Id);
        Task<bool> DeleteProcessFlowDiagram(Guid Id);
        Task<bool> DeleteProductionCapacityEstimation(Guid UserId, Guid id);
        // Task<bool> UploadSupportingEvidence(Guid UserId, IFormFile file, Guid PCEcaseId);
        // Task<bool> UploadProcessFlowDiagram(Guid UserId, IFormFile file, Guid PCEcaseId);
    }
}
