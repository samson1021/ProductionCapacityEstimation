using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEValuationHistoryDto
    {
        public PCEEvaluationReturnDto LatestEvaluation { get; set; }
        public IEnumerable<PCEEvaluationReturnDto> PreviousEvaluations { get; set; }
    }
}
