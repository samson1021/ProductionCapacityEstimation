using mechanical.Models.Dto.CaseAssignmentDto;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;

namespace mechanical.Services.CaseAssignmentService
{
    public interface ICaseAssignmentService
    {
        Task<List<CaseAssignmentDto>> SendForValuation(string selectedCollateralIds, string employeeId);
        Task<List<CaseAssignmentDto>> SendForReestimation(string ReestimationReason, string selectedCollateralIds, string CenterId);
        //Task<CaseAssignmentDto> CreateCaseAssignment(CaseAssignmentDto caseAssignmentDto);
        Task<List<CaseAssignmentDto>> AssignMakerTeamleader(Guid userId, string selectedCollateralIds, string employeeId);
        //Task<List<CaseAssignmentDto>> AssignMakerOfficer(Guid userId, string selectedCollateralIds, string employeeId);
        Task<List<CaseAssignmentDto>> ReAssignMakerTeamleader(Guid userId, string selectedCollateralIds, string employeeId);

        Task<List<CaseAssignmentDto>> AssignCheckerTeamleader(Guid userId, string selectedCollateralIds, string employeeId);
        //Task<List<CaseAssignmentDto>> AssignCheckerOfficer(string selectedCollateralIds, string employeeId);
        Task<List<CaseAssignmentDto>> ReAssignCheckerTeamleader(Guid userId, string selectedCollateralIds, string employeeId);
    }
}
