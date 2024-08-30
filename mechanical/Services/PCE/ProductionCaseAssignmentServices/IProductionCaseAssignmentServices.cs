using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;

namespace mechanical.Services.PCE.ProductionCaseAssignmentServices
{
    public interface IProductionCaseAssignmentServices
    {
        Task<List<ProductionCaseAssignmentDto>> SendProductionForValuation(string selectedProductionIds, string employeeId);
        Task<List<ProductionCaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string selectedProductionIds, string CenterId);
        
        Task<List<ProductionCaseAssignmentDto>> AssignProductMakerTeamleader(Guid userId, string selectedProductionIds, string employeeId);
        //Task<List<CaseAssignmentDto>> AssignMakerOfficer(Guid userId, string selectedCollateralIds, string employeeId);
        Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid userId, string selectedProductionIds, string employeeId);

        Task<List<ProductionCaseAssignmentDto>> AssignProductionCheckerTeamleader(Guid userId, string selectedProductionIds, string employeeId);
        //Task<List<CaseAssignmentDto>> AssignCheckerOfficer(string selectedCollateralIds, string employeeId);
        Task<List<ProductionCaseAssignmentDto>> ReAssignProductionCheckerTeamleader(Guid userId, string selectedProductionIds, string employeeId);
    }
}
