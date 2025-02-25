using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;

namespace mechanical.Services.MMCaseService
{
    public interface IMMCaseService
    {
        Task<IEnumerable<CaseDto>> GetMMNewCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetMTLCompletedCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetCMNewCases(Guid userId);
        
        Task<IEnumerable<CaseDto>> GetMyAssignmentCases(Guid userId);
        
        Task<IEnumerable<CaseDto>> GetMoRemarkedCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetMMPendingCases(Guid userId);
        //Task<IEnumerable<RMCaseDto>> GetMmPendingCases();
        //Task<IEnumerable<MMCaseDto>> GetMmLatestCases();
        //Task<CaseCountDto> GetDashboardCaseCount();
    }
}
