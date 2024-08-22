namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCECasesCountDto
    {
        public int NewPCECasesCount { get; set; }
        public int NewPCEsCount { get; set; }
        public int PendingPCECasesCount { get; set; }
        public int PendingPCEsCount { get; set; }
        public int EvaluatedPCECasesCount { get; set; }
        public int EvaluatedPCEsCount { get; set; }
        public int ReturnedPCECasesCount { get; set; }
        public int ReturnedPCEsCount { get; set; }
        public int ReevaluatedPCECasesCount { get; set; }
        public int ReevaluatedPCEsCount { get; set; }
        public int RejectedPCECasesCount { get; set; }
        public int RejectedPCEsCount { get; set; }
        public int TotalPCECasesCount { get; set; }
        public int TotalPCEsCount { get; set; }
    }
    public class PCEsCountDto
    {
        public int NewPCEsCount { get; set; }
        public int PendingPCEsCount { get; set; }
        public int EvaluatedPCEsCount { get; set; }
        public int ReturnedPCEsCount { get; set; }
        public int ReevaluatedPCEsCount { get; set; }
        public int RejectedPCEsCount { get; set; }
        public int TotalPCEsCount { get; set; }
    }
}
