using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid UserId, PCECaseDto caseDto);
        Task<PCECaseReturntDto> PCEEdit(Guid UserId, PCECaseReturntDto caseDto);
        

        Task<IEnumerable<PCENewCaseDto>> GetRmLatestPCECases(Guid UserId);
        Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid UserId);
        Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid UserId);
        Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid UserId);
        Task<IEnumerable<PCENewCaseDto>> GetPCERejectedCases(Guid UserId);
        Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid UserId);

        Task<PCECaseReturntDto> GetCase(Guid UserId, Guid Id);

        Task<PCECaseReturntDto> GetPCECaseDetail(Guid Id);
        Task<PCECaseReturntDto> GetCaseDetail(Guid Id);
        Task<PCEReportDataDto> GetPCEReportData(Guid Id);
        Task<PCEReportDataDto> GetPCEAllReportData(Guid Id);
        PCEReportDataDto GetPCECaseDetailReport(Guid UserId, Guid Id);
        Task<IEnumerable<PCENewCaseDto>> GetPCECasesReport(Guid UserId);

        Task<IEnumerable<PCENewCaseDto>> GetMyAssignmentPCECases(Guid UserId);   
        Task<IEnumerable<PCENewCaseDto>> GetRemarkedPCECases(Guid UserId);
        Task<PCECaseTerminate> ApproveCaseTermination(Guid Id);

        Task<PCECaseReturntDto> GetPCECase(Guid Id);
        Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id);
        Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null);
        Task<IEnumerable<PCENewCaseDto>> GetMyRemarkedPCECases(Guid UserId); 

        Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid UserId);
        Task<CreateNewCaseCountDto> GetMyDashboardCaseCount();
        Task<PCECasesCountDto> GetMyDashboardPCECasesCount(Guid UserId);
    }
}
