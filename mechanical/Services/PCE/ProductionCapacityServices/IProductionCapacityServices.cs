﻿
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCapacityServices
{
    public interface IProductionCapacityServices
    {
        Task<ProductionCapacity> CreateProductionCapacity(Guid userId, Guid caseId, ProductionPostDto createProductionDto);
        Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid CaseId);
        Task<ReturnProductionDto> GetProduction(Guid userId, Guid id);
        Task<bool> DeleteProduction(Guid userId, Guid id);
        Task<ProductionCapacity> EditProduction(Guid userId, Guid ProductionCapacityId, ProductionPostDto createProductionDto);
        Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid PCECaseId);
        Task<IEnumerable<ReturnProductionDto>> GetRemarkProducts(Guid userId, Guid PCECaseId);
        Task<ReturnProductionDto> GetProductionCapacityById(Guid productionid);

        Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid PCECaseId);
        Task<IEnumerable<ProductionCapcityCorrectionReturnDto1>> GetComments(Guid ProductionCapacityId);

        Task<bool> DeleteProductionFile(Guid userId, Guid Id);
        Task<bool> UploadProductionFile(Guid userId, IFormFile file, Guid caseId, string DocumentCatagory);



    }
}
