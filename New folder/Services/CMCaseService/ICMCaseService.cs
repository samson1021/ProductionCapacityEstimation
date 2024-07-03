using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;

namespace mechanical.Services.MMCaseService
{
    public interface ICMCaseService
    {
        Task<IEnumerable<MMNewCaseDto>> GetCMNewCases();
        Task<IEnumerable<RMCaseDto>> GetMmPendingCases();
        Task<IEnumerable<MMCaseDto>> GetMmLatestCases();
        Task<IEnumerable<CaseDto>> GetCoRemarkedCases(Guid userId);
        Task<CaseCountDto> GetDashboardCaseCount();
    }
}
