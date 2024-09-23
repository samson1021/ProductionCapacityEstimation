using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;

namespace mechanical.Services.PCE.PCECaseAssignmentServices
{
    public interface IPCECaseAssignmentServices
    {
        Task<List<PCECaseAssignmentDto>> SendProductionForValuation(string SelectedPCEIds, string EmployeeId);
        Task<List<PCECaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string SelectedPCEIds, string CenterId);
        
        Task<List<PCECaseAssignmentDto>> AssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId);
        Task<List<PCECaseAssignmentDto>> ReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId);

        // Task<List<PCECaseAssignmentDto>> AssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId);
        // Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId);

        // Task<List<PCECaseAssignmentDto>> AssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId);
        // Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId);
    }
}
