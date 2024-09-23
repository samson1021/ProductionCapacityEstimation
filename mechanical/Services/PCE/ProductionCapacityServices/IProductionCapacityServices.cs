using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;

namespace mechanical.Services.PCE.ProductionCapacityServices
{
    public interface IProductionCapacityServices
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid UserId, Guid caseId, ProductionPostDto createProductionDto);
        Task<ProductionCapacity> CreatePlantProduction(Guid UserId, Guid caseId, PlantPostDto createplantDto);
        Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid CaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendingProductions(Guid CaseId);
        Task<ReturnProductionDto> GetProduction(Guid UserId, Guid id);
        Task<PlantEditPostDto> GetPlantProduction(Guid UserId, Guid id);


        Task<bool> DeleteProduction(Guid UserId, Guid id);

        Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionPostDto createProductionDto);

        Task<ProductionCapacity> EditPlantProduction(Guid UserId, Guid ProductionCapacityId, PlantEditPostDto createProductionDto);


        Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmRejectedProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId);
        Task<ReturnProductionDto> GetProductionCapacityById(Guid productionid);
       
        Task<ReturnProductionDto> GetManufuctringProductionCapacityEvalutionById(Guid productionid);
        Task<PlantEditPostDto> GetPlantProductionCapacityEvalutionById(Guid productionid);
        Task<PCEEvaluationReturnDto> GetValuationById(Guid productionid);
        


        Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ProductionCapacityCorrectionReturnDto>> GetComments(Guid ProductionCapacityId);

        Task<bool> DeleteProductionFile(Guid UserId, Guid Id);
        Task<bool> UploadProductionFile(Guid UserId, IFormFile file, Guid caseId, string DocumentCatagory);

        Task<IEnumerable<ReturnProductionDto>> GetRmComCollaterals(Guid PCECaseId);


    }
}
