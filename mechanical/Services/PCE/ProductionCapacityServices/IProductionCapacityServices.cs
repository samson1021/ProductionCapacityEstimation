
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCapacityServices
{
    public interface IProductionCapacityServices
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid userId, Guid caseId, ProductionPostDto createProductionDto);
        Task<ProductionCapacity> CreatePlantProduction(Guid userId, Guid caseId, PlantPostDto createplantDto);
        Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid CaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendingProductions(Guid CaseId);
        Task<ReturnProductionDto> GetProduction(Guid userId, Guid id);
        Task<PlantEditPostDto> GetPlantProduction(Guid userId, Guid id);


        Task<bool> DeleteProduction(Guid userId, Guid id);

        Task<ProductionCapacity> EditProduction(Guid userId, Guid ProductionCapacityId, ProductionPostDto createProductionDto);

        Task<ProductionCapacity> EditPlantProduction(Guid userId, Guid ProductionCapacityId, PlantEditPostDto createProductionDto);


        Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmRejectedProductions(Guid userId, Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRemarkProducts(Guid userId, Guid PCECaseId);
        Task<ReturnProductionDto> GetProductionCapacityById(Guid productionid);

        Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ProductionCapcityCorrectionReturnDto1>> GetComments(Guid ProductionCapacityId);

        Task<bool> DeleteProductionFile(Guid userId, Guid Id);
        Task<bool> UploadProductionFile(Guid userId, IFormFile file, Guid caseId, string DocumentCatagory);

        Task<IEnumerable<ReturnProductionDto>> GetRmComCollaterals(Guid CaseId);


    }
}
