using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class ProductionLineEvaluationDto
    {
        public required string ProductionLineOrEquipmentName { get; set; }
        public required string OutputType { get; set; }
        public required string OutputQuantity { get; set; }
        public required string InputType { get; set; }
        public required string InputOutputConversion { get; set; }
        public OutputPhase OutputPhase { get; set; }
        public ProductionUnit ProductionUnit { get; set; }
    }
}
