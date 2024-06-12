﻿
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECase;
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


    }
}
