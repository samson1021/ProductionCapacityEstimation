using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using mechanical.Models.Dto.ProductionCapacityDto;


namespace mechanical.Services.ProductionCapacityService
{
    public interface IProductionCapacityEstimationService
    {

        Task<ProductionCapacityEstimationDto> CreateProductionCapacityEstimation(Guid userId, ProductionCapacityEstimationDto dto);
        Task<ProductionCapacityEstimationDto> EditProductionCapacityEstimation(Guid userId, Guid id, ProductionCapacityEstimationDto dto);
        Task<ProductionCapacityEstimationDto> GetProductionCapacityEstimation(Guid userId, Guid id);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetNewEstimations(Guid userId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetRejectedEstimations(Guid userId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetTerminatedEstimations(Guid userId);
        Task<IEnumerable<ProductionCapacityEstimationDto>> GetPendingEstimations(Guid userId);
        Task SendForApproval(string selectedEstimationIds, string CenterId);
        Task RejectEstimation(Guid id, string rejectionReason);
        Task<ProductionCapacityScheduleDto> CreateSchedule(Guid userId, ProductionCapacityScheduleDto scheduleDto);
        Task<int> GetDashboardEstimationCount(Guid userId);
        Task<int> GetMyDashboardEstimationCount(Guid userId);

        Task<IEnumerable<ProductionCapacityEstimationDto>> GetAllProductionCapacityEstimations();
        Task<bool> DeleteProductionCapacityEstimation(Guid userId, Guid id);
        
        Task<bool> DeleteSupportingEvidence(Guid Id);
        Task<bool> DeleteProcessFlowDiagram(Guid Id);
        Task<bool> UploadSupportingEvidence(Guid userId, IFormFile file, Guid caseId);
        Task<bool> UploadProcessFlowDiagram(Guid userId, IFormFile file, Guid caseId);
    }
}
