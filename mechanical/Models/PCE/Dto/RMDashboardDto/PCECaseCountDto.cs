namespace mechanical.Models.PCE.Dto.RMDashboardDto
{
    public class PCECaseCountDto
    {
        public int PCSNewCaseCount { get; set; }
        public int PCSPendingCaseCount { get; set; }
        public int PCSCompletedCaseCount { get; set; }
        public int PCSTotalCaseCount => PCSNewCaseCount + PCSPendingCaseCount + PCSCompletedCaseCount;

    }
}
