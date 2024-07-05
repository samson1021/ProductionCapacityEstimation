using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;

namespace mechanical.Services.PCE.PCECaseTimeLineService
{
    public interface IPCECaseTimeLineService
    {
        Task<PCECaseTimeLinePostDto> PCECaseTimeLine(PCECaseTimeLinePostDto pCECaseTimeLinePostDto);


        Task<IEnumerable<PCECaseTimeLineReturnDto>> GetPCECaseTimeLines(Guid CaseId);

    }
}
