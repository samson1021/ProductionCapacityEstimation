using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Entities;

namespace mechanical.Services.CaseServices
{
    public interface ICaseService
    {
        Task<Case> CreateCase(Guid userId, CasePostDto createCaseDto);
        Task<CaseReturnDto> GetCase(Guid userId, Guid id);
        Task<CaseReturnDto> GetShareTaskCase(Guid userId, Guid id);

        Task<string> GetCustomerName(double customerId);
        Task<CaseReturnDto> GetCaseDetail(Guid id);
        Task<IEnumerable<CaseDto>> GetNewCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetRejectedCases(Guid userId);
        Task<Case> EditCase(Guid userId, Guid id, CasePostDto createCaseDto);
        Task<IEnumerable<CaseDto>> GetRmLatestCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetMmLatestCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetMoLatestCases(Guid userId);
        Task<CaseCountDto> GetDashboardCaseCount(Guid userId);
        Task<CaseCountDto> GetMyDashboardCaseCount(Guid userId);
        Task<bool> SendRejection(MoRejectCaseDto moRejectCaseDto);
        Task<bool> RetrunToMaker(Guid Id);
        Task<CaseTerminate> ApproveCaseTermination(Guid id);
        Task<IEnumerable<CaseTerminateDto>> GetTerminatedCases(Guid userId);

        //Task<IEnumerable<RMCaseDto>> GetRmPendingCases();
        //Task<RmNewCaseDto> GetRmNewCase(Guid Id);

        Task<IEnumerable<CaseDto>> GetRmCompleteCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetRmTotalCases(Guid userId);
        
        Task<IEnumerable<CaseDto>> GetTotalCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetRmRemarkedCases(Guid userId);
        Task<bool> DeleteBussinessLicence(Guid Id);
        Task<bool> UploadBussinessLicence(Guid userId, IFormFile file, Guid caseId);
        Task<IEnumerable<CaseDto>> GetRmPendingCases(Guid userId);

        Task<Case> GetCaseById(Guid caseId);
        Task<IEnumerable<CaseDto>> GetMyCases(Guid userId, string Status = "", int? Limit = null);
        Task<IEnumerable<CaseDto>> GetReceivedCases(Guid userId, string Status = "", int? Limit = null);
        Task<IEnumerable<CaseDto>> GetSharedCases(Guid userId, string Status = "", int? Limit = null);

        // shared case info
        Task<ShareTasksDto> SharedCaseInfo(Guid id);

        Task<IEnumerable<CaseDto>> GetTotalHOPendingCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetTotalHONewCases(Guid userId);
        Task<CaseReturnDto> GetHOCase(Guid id);
        Task<CaseReturnDto> GetHOPendingCase(Guid userId, Guid id);
        Task<IEnumerable<CaseDto>> GetHOCompleteCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetHOLatestCases(Guid userId);
        
    }
}
