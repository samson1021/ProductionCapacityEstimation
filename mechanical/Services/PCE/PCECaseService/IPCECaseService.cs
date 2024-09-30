using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid UserId, PCECaseDto caseDto);
        Task<PCECaseReturnDto> PCEEdit(Guid UserId, PCECaseReturnDto caseDto);
        

        Task<IEnumerable<PCECaseReturnDto>> GetRmLatestPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCENewCases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCEPendingCases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCECompleteCases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCERejectedCases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCETotalCases(Guid UserId);

        Task<PCECaseReturnDto> GetCase(Guid UserId, Guid Id);

        Task<PCECaseReturnDto> GetPCECaseDetail(Guid Id);
        Task<PCECaseReturnDto> GetCaseDetail(Guid Id);
        Task<PCEReportDataDto> GetPCEReportData(Guid Id);
        Task<PCEReportDataDto> GetPCEAllReportData(Guid Id);
        PCEReportDataDto GetPCECaseDetailReport(Guid UserId, Guid Id);
        Task<IEnumerable<PCECaseReturnDto>> GetPCECasesReport(Guid UserId);

        Task<IEnumerable<PCECaseReturnDto>> GetMyAssignmentPCECases(Guid UserId);   
        Task<IEnumerable<PCECaseReturnDto>> GetRemarkedPCECases(Guid UserId);
        Task<PCECaseTerminate> ApproveCaseTermination(Guid Id);

        Task<PCECaseReturnDto> GetPCECase(Guid Id);
        Task<PCECaseReturnDto> GetPCECase(Guid UserId, Guid Id);
        Task<IEnumerable<PCECaseReturnDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null);
        Task<IEnumerable<PCECaseReturnDto>> GetMyRemarkedPCECases(Guid UserId); 

        Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid UserId);
        Task<CreateNewCaseCountDto> GetMyDashboardCaseCount();
        Task<PCECasesCountDto> GetMyDashboardPCECasesCount(Guid UserId);
    }
}
