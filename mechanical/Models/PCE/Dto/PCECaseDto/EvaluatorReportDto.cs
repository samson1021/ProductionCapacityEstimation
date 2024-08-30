namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class EvaluatorReportDto
    {
        public Guid? EvaluatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? EvaluatorName { get; set; }
        public Guid? PCEvaluationId { get; set; }
        public Guid? SignatureImageId { get; set; }
    }
}
