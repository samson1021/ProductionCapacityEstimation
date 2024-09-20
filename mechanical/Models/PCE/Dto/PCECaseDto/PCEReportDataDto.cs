using mechanical.Models.PCE.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCEReportDataDto
    {
        public PCECase PCESCase { get; set; }
        public List<ProductionCapacity> Productions { get; set; }
        public List<PCEEvaluation> PCEEvaluations { get; set; }
        public PCECaseSchedule PCECaseSchedule { get; set; }
    }
}