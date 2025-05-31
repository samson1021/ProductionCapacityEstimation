


using mechanical.Models.Dto.InternalReport;

namespace mechanical.Services.InternalReportService
{
    public interface IInternalReportService
    {
        Task<IEnumerable<InternalCaseReportDto>> GetInternalCaseReport(Guid userId);
        Task<(IEnumerable<ValuationReportDto> DistinctCases, IEnumerable<ValuationReportDto> AllProductionCapacities)> GetInternalPCECaseReport(Guid userId);

    }
}
