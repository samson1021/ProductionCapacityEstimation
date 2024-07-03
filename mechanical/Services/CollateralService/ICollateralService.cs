using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;

namespace mechanical.Services.CollateralService
{
    public interface ICollateralService
    {
        Task<Collateral> CreateCollateral(Guid userId, Guid caseId, CollateralPostDto createCollateralDto);
        Task<Collateral> CreateCivilCollateral(Guid userId, Guid caseId, CivilCollateralPostDto createCivilCollateralDto);
        Task<Collateral> CreateAgricultureCollateral(Guid userId, Guid caseId, AgricultureCollateralPostDto createAgricultureCollateralDto);
        Task<IEnumerable<ReturnCollateralDto>> GetCollaterals(Guid CaseId);
        Task<ReturnCollateralDto> GetCollateral(Guid userId,Guid id);
        Task<bool> DeleteCocllateral(Guid userId,Guid id);
        Task<Collateral> EditCollateral(Guid userId, Guid CollaterlId, CollateralPostDto createCollateralDto);

        Task<IEnumerable<ReturnCollateralDto>> GetRejectedCollaterals(Guid CaseId);
        Task<IEnumerable<ReturnCollateralDto>> GetPendCollaterals(Guid CaseId);
        Task<IEnumerable<ReturnCollateralDto>> GetRmComCollaterals(Guid CaseId);
        Task<IEnumerable<CollateralAssignmentDto>> GetMyAssignmentCollateral(Guid UserId , Guid CaseId);
        //Task<ReturnCollateralDto> GetCollateral(Guid Id);
        //Task<IEnumerable<ReturnCollateralDto>> GetCompleteCollaterals(Guid CaseId);

        // Task<IEnumerable<ReturnCollateralDto>> GetMOCollaterals(Guid CaseId);
        // Task<IEnumerable<ReturnCollateralDto>> GetCOCollaterals(Guid CaseId);
        Task<IEnumerable<ReturnCollateralDto>> GetMMCollaterals(Guid userId, Guid CaseId);
        Task<IEnumerable<ReturnCollateralDto>> GetCMCollaterals(Guid userId, Guid CaseId);
        Task<IEnumerable<ReturnCollateralDto>> GetRemarkCollaterals(Guid userId, Guid CaseId);

        Task<IEnumerable<ReturnCollateralDto>> GetMMPendCollaterals(Guid userId, Guid CaseId);
        // Task<IEnumerable<ReturnCollateralDto>> GetMTLCollaterals(Guid CaseId);
        // Task<IEnumerable<ReturnCollateralDto>> GetCTLCollaterals(Guid CaseId);
        // Task<ReturnCollateralDto> GetMORetunedCollaterals(Guid CaseId);
        Task<Collateral> ChangeStatus(Guid useId,Guid CaseId, string Status);
        // Task<IEnumerable<ReturnCollateralDto>> GetCMCollaterals(Guid CaseId);
        // Task<IEnumerable<ReturnFileDto>> GetCollateralFile(Guid CollateralId);


        Task<IEnumerable<ReturnCollateralDto>> MyReturnedCollaterals(Guid userId);
        Task<IEnumerable<ReturnCollateralDto>> MyResubmitedCollaterals(Guid userId);
        Task<ReturnCollateralDto> MyResubmitedCollateral(Guid userId, Guid id);
        Task<ReturnCollateralDto> MyReturnedCollateral(Guid userId, Guid id);
        
        Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid CollateralId);
        Task<bool> DeleteCollateralFile(Guid userId, Guid Id);
        Task<bool> UploadCollateralFile(Guid userId, IFormFile file, Guid caseId, string DocumentCatagory);

    }
}
