


using mechanical.Models.Dto.InternalReport;

namespace mechanical.Services.InternalReportService
{
    public interface IInternalReportService
    {
        Task<IEnumerable<InternalCaseReportDto>> GetCaseReport(Guid userId);
    
    }
}
