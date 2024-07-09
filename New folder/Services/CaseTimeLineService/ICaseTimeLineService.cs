using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.CollateralDto;

namespace mechanical.Services.CaseTimeLineService
{
    public interface ICaseTimeLineService
    {
        Task<CaseTimeLineDto> CreateCaseTimeLine(CaseTimeLinePostDto caseTimeLinePostDto);
        Task<IEnumerable<CaseTimeLineReturnDto>> GetCaseTimeLines(Guid CaseId);
    }
}
