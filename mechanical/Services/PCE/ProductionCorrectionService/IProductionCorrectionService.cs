


using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public interface IProductionCorrectionService
    {
        Task<ProductionCapacityCorrectionReturnDto> CreateProductionCorrection(ProductionCapacityCorrectionPostDto correctionDto);
        Task<ProductionCapacityCorrectionPostDto> GetProductionCorrection(Guid Id);
    }
}
