namespace mechanical.Models.PCE.Dto
{
    public class CreateNewCaseCountDto
    {
        public int PCSNewCaseCount { get; set; }
        public int PCSPendingCaseCount { get; set; }
        public int PCSCompletedCaseCount { get; set; }
        public int PCSTotalCaseCount => PCSNewCaseCount + PCSPendingCaseCount + PCSCompletedCaseCount;
    }
}
