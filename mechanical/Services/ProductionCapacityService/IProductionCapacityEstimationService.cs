using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using mechanical.Models.Dto.ProductionCapacityDto;

namespace mechanical.Services.ProductionCapacityService
{
    public interface IProductionCapacityEstimationService
    {
        Task<ProductionCapacityEstimationDto> CreateProductionCapacityEstimation(Guid UserId, Guid PCEcaseId, ProductionCapacityEstimationDto dto);
        Task<ProductionCapacityEstimationDto> EditProductionCapacityEstimation(Guid UserId, Guid id, ProductionCapacityEstimationDto dto);
        Task<ProductionCapacityEstimationDto> GetProductionCapacityEstimation(Guid UserId, Guid id);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetAllProductionCapacityEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetNewEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetRejectedEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetTerminatedEstimations(Guid UserId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetPendingEstimations(Guid UserId);
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
