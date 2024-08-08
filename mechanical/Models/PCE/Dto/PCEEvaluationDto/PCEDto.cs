using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEDto
    {
        public ReturnProductionDto ProductionCapacity { get; set; }
        public PCECase PCECase { get; set; }
        public IEnumerable<ReturnFileDto> RelatedFiles { get; set; }
        public CreateUser CurrentUser { get; set; }
        public ProductionCapacityReestimation Reestimation { get; set; }
        public PCEEvaluationReturnDto LatestPCEEvaluation { get; set; }
    }
}
