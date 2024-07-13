

using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECollateralService
{

    public interface IPCECollateralService
    {
         Task<ProductionCapacity> CreatePCECollateral(PlantCapacityEstimationPostDto plantCapacityEstimationPostDto);

        Task<IEnumerable<PCEReturnCollateralDto>> GetCollaterals(Guid CaseId);
        Task<PCEReturnCollateralDto> GetCollateral(Guid id);

        Task<ProductionCapacity> EditCollateral(Guid userId, Guid CollaterlId, PlantCapacityEstimationEditPostDto createCollateralDto);

        Task<bool> DeleteCollateralFile(Guid userId, Guid Id);

        Task<bool> UploadCollateralFile(Guid userId, IFormFile file, Guid pcecaseId, string DocumentCatagory, Guid caseId);


        Task<bool> DeleteCocllateral(Guid userId, Guid id);



    }
}
