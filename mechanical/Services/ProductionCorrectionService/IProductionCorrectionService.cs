using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.Entities;

namespace mechanical.Services.ProductionCorrectionService
{
    public interface IProductionCorrectionService
    {
        Task<ProductionCapcityCorrection> CreateProductionCorrection(ProductionCapcityCorrectionPostDto correctionDto);
        Task<ProductionCapcityCorrectionPostDto> GetProductionCorrection(Guid Id);
    }
}
