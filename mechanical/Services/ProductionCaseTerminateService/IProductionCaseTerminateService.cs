using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Models.Dto.ProductionCaseTerminateDto;

namespace mechanical.Services.ProductionCaseTerminateService
{
    public interface IProductionCaseTerminateService
    {
        Task<ProductionCaseTerminateReturnDto> CreateProductionCaseTerminate(Guid userId, ProductionCaseTerminateReturnDto caseTerminatePostDto);
        Task<ProductionCaseTerminateReturnDto> UpdateProductionCaseTerminate(Guid userId, Guid id, ProductionCaseTerminateReturnDto caseTerminatePostDto);
        Task<ProductionCaseTerminateReturnDto> ApproveProductionCaseTerminate(Guid id);
        Task<IEnumerable<ProductionCaseTerminateReturnDto>> GetProductionCaseTerminates(Guid caseId);
    }
}
