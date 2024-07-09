


using mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public interface IProductionCorrectionService
    {
        Task<ProductionCapcityCorrection> CreateProductionCorrection(ProductionCapcityCorrectionPostDto1 correctionDto);
        Task<ProductionCapcityCorrectionPostDto1> GetProductionCorrection(Guid Id);
    }
}
