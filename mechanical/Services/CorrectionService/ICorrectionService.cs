using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Entities;

namespace mechanical.Services.CorrectionServices
{
    public interface ICorrectionService
    {
        Task<Correction> CreateCorrection(CorrectionPostDto correctionDto);
        Task<CorrectionPostDto> GetCorrection(Guid Id);
    }
}
