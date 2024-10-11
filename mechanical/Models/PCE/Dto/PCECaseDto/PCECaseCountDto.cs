namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCECaseCountDto
    {
        public int NewPCECaseCount { get; set; }
        public int NewProductionCount { get; set; }
        public int PendingPCECaseCount { get; set; }
        public int PendingProductionCount { get; set; }
        public int CompletedPCECaseCount { get; set; }
        public int CompletedProductionCount { get; set; }
        public int ResubmittedPCECaseCount { get; set; }
        public int ResubmittedProductionCount { get; set; }
        public int ReestimatedPCECaseCount { get; set; }
        public int ReestimatedProductionCount { get; set; }
        public int RejectedPCECaseCount { get; set; }
        public int RejectedProductionCount { get; set; }
        public int TotalPCECaseCount { get; set; }
        public int TotalProductionCount { get; set; }
    }

    public class ProductionCountDto
    {
        public int NewProductionCount { get; set; }
        public int PendingProductionCount { get; set; }
        public int CompletedProductionCount { get; set; }
        public int ResubmittedProductionCount { get; set; }
        public int ReestimatedProductionCount { get; set; }
        public int RejectedProductionCount { get; set; }
        public int TotalProductionCount { get; set; }
    }    
}
