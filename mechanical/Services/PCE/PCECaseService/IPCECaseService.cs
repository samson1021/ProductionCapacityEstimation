
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.PCECaseService
{
    public interface IPCECaseService
    {
        Task<PCECase> PCECase(Guid userId, PCECaseDto caseDto);
        Task<CreateNewCaseCountDto> GetDashboardPCECaseCount();
        Task<CreateNewCaseCountDto> GetMyDashboardCaseCount();

        Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId);
        Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId);


        PCECaseReturntDto GetPCECase(Guid userId, Guid id);


      Task<PCECaseReturntDto> PCEEdit(Guid userId, PCECaseReturntDto caseDto);

        ////manufuctuer
        Task<PCECaseReturntDto> GetProductionCaseDetail(Guid id);

        Task<PCECaseReturntDto> GetCase(Guid userId, Guid id);


    }
}
