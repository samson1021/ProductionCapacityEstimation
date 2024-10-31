using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;

namespace mechanical.Services.PCE.PCECaseAssignmentService
{
    public interface IPCECaseAssignmentService
    {
        Task<List<PCECaseAssignmentDto>> SendForValuation(Guid UserId, string SelectedPCEIds, string CenterId);
        Task<List<PCECaseAssignmentDto>> SendForReestimation(Guid UserId, string ReestimationReason, string SelectedPCEIds, string CenterId);
        Task<List<PCECaseAssignmentDto>> AssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId);
        Task<List<PCECaseAssignmentDto>> ReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId);
    }
}
