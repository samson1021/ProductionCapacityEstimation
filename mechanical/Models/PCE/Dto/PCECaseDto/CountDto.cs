namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECasesCountDto
    {
        public int NewPCECasesCount { get; set; }
        public int NewProductionsCount { get; set; }
        public int PendingPCECasesCount { get; set; }
        public int PendingProductionsCount { get; set; }
        public int CompletedPCECasesCount { get; set; }
        public int CompletedProductionsCount { get; set; }
        public int ResubmittedPCECasesCount { get; set; }
        public int ResubmittedProductionsCount { get; set; }
        public int ReestimatedPCECasesCount { get; set; }
        public int ReestimatedProductionsCount { get; set; }
        public int RejectedPCECasesCount { get; set; }
        public int RejectedProductionsCount { get; set; }
        public int TotalPCECasesCount { get; set; }
        public int TotalProductionsCount { get; set; }
    }
    public class ProductionsCountDto
    {
        public int NewProductionsCount { get; set; }
        public int PendingProductionsCount { get; set; }
        public int CompletedProductionsCount { get; set; }
        public int ResubmittedProductionsCount { get; set; }
        public int ReestimatedProductionsCount { get; set; }
        public int RejectedProductionsCount { get; set; }
        public int TotalProductionsCount { get; set; }
    }
}
