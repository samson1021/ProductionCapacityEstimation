namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECasesCountDto
    {
        public int NewPCECasesCount { get; set; }
        public int NewPCEsCount { get; set; }
        public int PendingPCECasesCount { get; set; }
        public int PendingPCEsCount { get; set; }
        public int CompletedPCECasesCount { get; set; }
        public int CompletedPCEsCount { get; set; }
        public int ResubmittedPCECasesCount { get; set; }
        public int ResubmittedPCEsCount { get; set; }
        public int ReestimatedPCECasesCount { get; set; }
        public int ReestimatedPCEsCount { get; set; }
        public int RejectedPCECasesCount { get; set; }
        public int RejectedPCEsCount { get; set; }
        public int TotalPCECasesCount { get; set; }
        public int TotalPCEsCount { get; set; }
    }
    public class PCEsCountDto
    {
        public int NewPCEsCount { get; set; }
        public int PendingPCEsCount { get; set; }
        public int CompletedPCEsCount { get; set; }
        public int ResubmittedPCEsCount { get; set; }
        public int ReestimatedPCEsCount { get; set; }
        public int RejectedPCEsCount { get; set; }
        public int TotalPCEsCount { get; set; }
    }
}
