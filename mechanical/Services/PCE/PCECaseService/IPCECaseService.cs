using mechanical.Models.Dto.CaseDto;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid userId, PCECaseDto caseDto);
        Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid userId);
        Task<CreateNewCaseCountDto> GetMyDashboardCaseCount();

        Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCECasesReport(Guid userId);

        Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCERejectedCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId);

        Task<IEnumerable<PCENewCaseDto>> GetRmLatestPCECases(Guid userId);

        PCECaseReturntDto GetPCECase(Guid userId, Guid id);
        PCEReportDataDto GetPCECaseDetailReport(Guid userId, Guid id);

        Task<PCECaseReturntDto> PCEEdit(Guid userId, PCECaseReturntDto caseDto);

        ////manufuctuer
        Task<PCECaseReturntDto> GetProductionCaseDetail(Guid id);

        Task<PCECaseReturntDto> GetCase(Guid userId, Guid id);

        Task<PCEReportDataDto> GetPCEReportData(Guid Id);
        Task<PCEReportDataDto> GetPCEAllReportData(Guid Id);

        Task<IEnumerable<PCENewCaseDto>> GetMyAssignmentPCECases(Guid UserId);   
        Task<IEnumerable<PCENewCaseDto>> GetRemarkedPCECases(Guid UserId);
        Task<IEnumerable<PCECaseTerminateDto>> GetCaseTerminates(Guid userId);
        Task<PCECaseReturntDto> GetCaseDetail(Guid id);
        Task<PCECaseTerminate> ApproveCaseTermination(Guid id);
    }
}
