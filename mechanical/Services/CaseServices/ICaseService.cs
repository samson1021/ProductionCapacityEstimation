using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.Entities;

namespace mechanical.Services.CaseServices
{
    public interface ICaseService
    {
        Task<Case> CreateCase(Guid userId, CasePostDto createCaseDto);
        Task<CaseReturntDto> GetCase(Guid userId, Guid id);
        Task<CaseReturntDto> GetCaseDetail(Guid id);
        Task<IEnumerable<CaseDto>> GetNewCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetRejectedCases(Guid userId);
        Task<Case> EditCase(Guid userId, Guid id, CasePostDto createCaseDto);
        Task<IEnumerable<CaseDto>> GetRmLatestCases(Guid userId);
        Task<IEnumerable<CaseDto>> GetMmLatestCases(Guid userId);
        Task<CaseCountDto> GetDashboardCaseCount(Guid userId);
        Task<CaseCountDto> GetMyDashboardCaseCount(Guid userId);
        Task<bool> SendRejection(MoRejectCaseDto moRejectCaseDto);
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


        //Task<IEnumerable<RMCaseDto>> GetMTLPendingCases();

        //Task<IEnumerable<MMNewCaseDto>> GetCheckerNewCases();


        //Task<IEnumerable<MMNewCaseDto>> GetMTLNewCases();
        //Task<IEnumerable<CONewCaseDto>> GetCONewCases();
        //Task<RmNewCaseDto> GetCONewCase(Guid Id);
        //Task<bool> SendEvaluation(Guid CaseId);
        //Task<bool> CheckedAndSendToRM(Guid CaseId);
        //Task<bool> CheckedAndRejectedSendToMO(Guid CaseId);

        //Task<Collateral> MyReturnedCases();
        //Task<Collateral> MyResubmitedCases();
        //Task<Collateral> MyReturnedCase(Guid CollateralId);
        //Task<Collateral> MyResubmitedCase(Guid CollateralId);




    }
}
