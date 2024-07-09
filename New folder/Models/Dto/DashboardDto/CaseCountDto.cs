namespace mechanical.Models.Dto.DashboardDto
{
    public class CaseCountDto
    {
        public int NewCaseCount { get; set; }
        public int NewCollateralCount { get; set; }
        public int PendingCaseCount { get; set; }
        public int PendingCollateralCount { get; set; }
        public int CompletedCaseCount { get; set; }
        public int CompletedCollateralCount { get; set; }
        public int TotalCaseCount { get; set; }
        public int TotalCollateralCount { get; set; }
    }
}
