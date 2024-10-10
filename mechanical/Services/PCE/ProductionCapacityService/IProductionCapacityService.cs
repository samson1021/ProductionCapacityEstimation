using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;

namespace mechanical.Services.PCE.ProductionCapacityService
{
    public interface IProductionCapacityService
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid UserId, Guid caseId, ProductionPostDto createProductionDto);
        Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionPostDto createProductionDto);
        Task<bool> DeleteProduction(Guid UserId, Guid Id);
        
        Task<bool> DeleteProductionFile(Guid UserId, Guid Id);
        Task<bool> UploadProductionFile(Guid UserId, IFormFile file, Guid caseId, string DocumentCatagory);
        
        Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid Id);
        Task<ReturnProductionDto> GetProduction(Guid UserId, Guid Id);
        Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        Task<int> GetProductionsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
       
        Task<PlantEditPostDto> GetPlantProduction(Guid UserId, Guid Id);
        Task<ProductionCapacity> CreatePlantProduction(Guid UserId, Guid caseId, PlantPostDto createplantDto);
        Task<ProductionCapacity> EditPlantProduction(Guid UserId, Guid ProductionCapacityId, PlantEditPostDto createProductionDto);

        Task<IEnumerable<ReturnProductionDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ProductionAssignmentDto>> GetAssignedProductions(Guid UserId, Guid PCECaseId);
         
        // Task<IEnumerable<ProductionCapacityCorrectionReturnDto>> GetComments(Guid ProductionCapacityId);

        // Task<IEnumerable<ReturnProductionDto>> GetPendingProductions(Guid CaseId);
        // Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid PCECaseId);
        // Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid PCECaseId);
        // Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid PCECaseId);
        // Task<IEnumerable<ReturnProductionDto>> GetRmRejectedProductions(Guid UserId, Guid PCECaseId);
        // Task<PCEEvaluationReturnDto> GetValuationById(Guid productionId);
        // Task<PlantEditPostDto> GetPlantProductionCapacityEvalutionById(Guid productionId);
        // Task<ReturnProductionDto> GetManufuctringProductionCapacityEvalutionById(Guid productionId);
    
        // Task<int> GetProductionsCount(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null);
        // Task<ProductionsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null);
    }
}
