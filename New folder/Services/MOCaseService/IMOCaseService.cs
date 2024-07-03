using mechanical.Models.Dto.CaseDto;
namespace mechanical.Services.MOCaseService
{
    public interface ICOCaseService
    {
        //Task<IEnumerable<MMNewCaseDto>> GetMONewCases();
        Task<bool> SendCheking(Guid userId,Guid CollateralId);
    }
}
