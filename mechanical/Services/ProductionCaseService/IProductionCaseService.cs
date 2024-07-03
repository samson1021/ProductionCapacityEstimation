

using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.ProductionCaseDto;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using mechanical.Models.Entities;
namespace mechanical.Services.ProductionCaseService
{
    public interface IProductionCaseService
    {
        Task<ProductionCase> CreateProductionCase(Guid userId, ProductionCasePostDto createCaseDto);
        Task<RetunProductionCaseDto> GetProductionCaseDetail(Guid id); 
        Task<IEnumerable<ProductionCaseDto>> GetNewProductionCases(Guid userId);
        Task<RetunProductionCaseDto> GetProductionCase(Guid userId, Guid id);
        Task<IEnumerable<ProductionCaseDto>> GetProductionRmLatestCases(Guid userId);
        Task<IEnumerable<ProductionCaseDto>> GetProductionMmLatestCases(Guid userId);
        Task<ProductionCaseTerminate> ApproveProductionCaseTermination(Guid id);
        Task<bool> DeleteProuctionBussinessLicence(Guid Id);
        Task<bool> UploadProductionBussinessLicence(Guid userId, IFormFile file, Guid caseId);
        Task<ProductionCase> EditProductionCase(Guid userId, Guid id, ProductionCasePostDto createCaseDto);

        

    }
}
