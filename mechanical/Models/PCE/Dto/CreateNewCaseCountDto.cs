namespace mechanical.Models.PCE.Dto
{
    public class CreateNewCaseCountDto
    {
        public int PCSNewCaseCount { get; set; }
        public int PCSPendingCaseCount { get; set; }
        public int PCSCompletedCaseCount { get; set; }
        public int PCSTotalCaseCount => PCSNewCaseCount + PCSPendingCaseCount + PCSCompletedCaseCount;
    }
    
    public class MyPCECaseCountDto
    {
        public int NewPCECaseCount { get; set; }
        public int NewPCECollateralCount { get; set; }
        public int PendingPCECaseCount { get; set; }
        public int PendingPCECollateralCount { get; set; }
        public int CompletedPCECaseCount { get; set; }
        public int CompletedPCECollateralCount { get; set; }
        public int TotalPCECaseCount { get; set; }
        public int TotalPCECollateralCount { get; set; }
    }
}
