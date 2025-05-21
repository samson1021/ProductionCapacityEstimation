using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Services.PCE.ProductionCapacityService
{
    public interface IProductionCapacityService
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid UserId, Guid PCECaseId, ProductionPostDto createProductionDto);
        Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionEditDto createProductionDto);
        Task<bool> DeleteProduction(Guid UserId, Guid Id);
        
        Task<bool> DeleteProductionFile(Guid UserId, Guid Id);
        Task<bool> UploadProductionFile(Guid UserId, IFormFile File, Guid ProductionId, string DocumentCategory);
        
        Task<ProductionDetailDto> GetProductionDetails(Guid UserId, Guid Id);
        Task<ProductionReturnDto> GetProduction(Guid UserId, Guid Id);
        Task<IEnumerable<ProductionReturnDto>> GetProductions(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        Task<int> GetProductionCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);

        Task<IEnumerable<ProductionReturnDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ProductionAssignmentDto>> GetAssignedProductions(Guid UserId, Guid PCECaseId);
    }
}
