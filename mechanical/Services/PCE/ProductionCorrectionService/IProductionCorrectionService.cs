using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public interface IProductionCorrectionService
    {
        Task<ProductionCapacityCorrectionReturnDto> CreateProductionCorrection(ProductionCapacityCorrectionPostDto correctionDto);
        Task<ProductionCapacityCorrectionPostDto> GetProductionCorrection(Guid Id);
    }
}
