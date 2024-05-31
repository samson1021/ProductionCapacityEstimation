namespace mechanical.Models.Dto.CaseDto
{
    public class MoRejectCaseDto
    {
        public required Guid CollateralId { get; set; }
        public required string RejectionComment { get; set; }
        
    }
}
