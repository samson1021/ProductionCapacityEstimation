
using mechanical.Models.PCE.Dto;
<<<<<<< HEAD
using mechanical.Models.PCE.Dto.PCECaseDto;
=======
using mechanical.Models.PCE.Dto.PCECase;
>>>>>>> c817e06b5076dffc526c8104a3cc2feb6aca029a
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid userId, PCECaseDto caseDto);
        Task<CreateNewCaseCountDto> GetDashboardPCSCaseCount();

        Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId);


        PCECaseReturntDto GetPCECase(Guid userId, Guid id);


      Task<PCECaseReturntDto> PCEEdit(Guid userId, PCECaseReturntDto caseDto);

<<<<<<< HEAD
=======
        ////manufuctuer
        Task<PCECaseReturntDto> GetProductionCaseDetail(Guid id);

        //Task<PCECase> CreateProductionCase(Guid userId, PCECaseDto createCaseDto);
        //Task<PCECaseReturntDto> GetProductionCaseDetail(Guid id);

>>>>>>> c817e06b5076dffc526c8104a3cc2feb6aca029a

    }
}
