using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid UserId, PCECaseDto caseDto);
        Task<PCECaseReturnDto> Edit(Guid UserId, PCECaseReturnDto caseDto);       

        Task<IEnumerable<PCECaseReturnDto>> GetLatestPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetAssignedPCECases(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetRemarkedPCECases(Guid UserId);

        Task<PCECaseReturnDto> GetPCECase(Guid UserId, Guid Id);
        Task<PCECaseCountDto> GetDashboardPCECaseCount(Guid UserId);
        Task<IEnumerable<PCECaseReturnDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null);


        Task<PCEReportDatalastDto> GetPCEReportData(Guid Id);
        Task<PCEReportDataDto> GetPCEAllReportData(Guid Id);
        Task<PCEReportDataDto> GetPCECaseDetailReport(Guid UserId, Guid Id);
        Task<IEnumerable<PCECaseReturnDto>> GetPCECasesReport(Guid UserId);
    }
}
