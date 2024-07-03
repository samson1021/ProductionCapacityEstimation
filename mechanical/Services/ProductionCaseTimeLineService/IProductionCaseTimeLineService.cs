using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;

namespace mechanical.Services.ProductionCaseTimeLineService
{
    public interface IProductionCaseTimeLineService
    {
        Task<ProductionCaseTimeLineDto> CreateProductionCaseTimeLine(ProductionCaseTimeLinePostDto caseTimeLinePostDto);
        Task<IEnumerable<ProductionCaseTimeLineReturnDto>> GetProductionCaseTimeLines(Guid CaseId);

    }
}

