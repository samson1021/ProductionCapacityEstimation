using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.Entities;

namespace mechanical.Services.ProductionCapacityServices
{
    public interface IProductionCapacityServices
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid userId, Guid caseId, ProductionPostDto createProductionDto);
        Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid ProductionCaseId);
        Task<ReturnProductionDto> GetProduction(Guid userId, Guid id);
        Task<bool> DeleteProduction(Guid userId, Guid id);
        Task<ProductionCapacity> EditProduction(Guid userId, Guid ProductionCapacityId, ProductionPostDto createProductionDto);
        Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid CaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid CaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid CaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRemarkProducts(Guid userId, Guid CaseId);
        Task<ReturnProductionDto> GetProductionCapacityById(Guid productionid);

        Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid CaseId);
        Task<IEnumerable<ProductionCapcityCorrectionReturnDto>> GetComments(Guid CollateralId);

        Task<bool> DeleteProductionFile(Guid userId, Guid Id);
        Task<bool> UploadProductionFile(Guid userId, IFormFile file, Guid caseId, string DocumentCatagory);
      


    }
}
