using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;

namespace mechanical.Models.PCE.Dto.ProductionCapacityDto
{
    public class PCEDetailDto
    {
        public ReturnProductionDto ProductionCapacity { get; set; }
        public PCECase PCECase { get; set; }
        public IEnumerable<ReturnFileDto> RelatedFiles { get; set; }
        public ProductionCapacityReestimation Reestimation { get; set; }
        public PCEValuationHistoryDto PCEValuationHistory { get; set; }
        public ProductionReject RejectedProduction { get; set; }
        public CreateUser RejectedBy { get; set; }
    }    
}
