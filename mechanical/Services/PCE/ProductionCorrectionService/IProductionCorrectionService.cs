


using mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public interface IProductionCorrectionService
    {
        Task<ProductionCapcityCorrection> CreateProductionCorrection(ProductionCapcityCorrectionPostDto correctionDto);
        Task<ProductionCapcityCorrectionPostDto> GetProductionCorrection(Guid Id);
    }
}
