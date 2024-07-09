using mechanical.Models.Dto.CaseDto;

namespace mechanical.Services.CTLCaseService
{
    public interface ICTLCaseService
    {
        Task<IEnumerable<RMCaseDto>> GetCTLPendingCases();
        Task<IEnumerable<MMNewCaseDto>> GetCTLNewCases();
    }
}
