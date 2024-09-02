using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;

namespace mechanical.Services.PCE.ProductionCaseAssignmentServices
{
    public interface IProductionCaseAssignmentServices
    {
        Task<List<ProductionCaseAssignmentDto>> SendProductionForValuation(string SelectedPCEIds, string EmployeeId);
        Task<List<ProductionCaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string SelectedPCEIds, string CenterId);
        
        Task<List<ProductionCaseAssignmentDto>> AssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId);
        //Task<List<CaseAssignmentDto>> AssignMakerOfficer(Guid UserId, string selectedCollateralIds, string EmployeeId);
        Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId);

        Task<List<ProductionCaseAssignmentDto>> AssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId);
        Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId);
    }
}
