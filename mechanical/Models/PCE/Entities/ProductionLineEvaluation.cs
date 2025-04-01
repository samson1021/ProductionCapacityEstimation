using mechanical.Models.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Entities
{
    public class ProductionLineEvaluation
    {
        public Guid Id { get; set; }
        public required Guid PCEEvaluationId { get; set; }
        public required Guid EvaluatorId { get; set; }

        public required string ProductionLineOrEquipmentName { get; set; }
        public required string OutputType { get; set; }
        public required string OutputQuantity { get; set; }
        public required string InputType { get; set; }
        public required string InputOutputConversion { get; set; }
        public OutputPhase OutputPhase { get; set; }
        public ProductionUnit ProductionUnit { get; set; }



        public virtual PCEEvaluation? PCEEvaluation { get; set; }
        public virtual CreateUser? Evaluator { get; set; }
    }
}
