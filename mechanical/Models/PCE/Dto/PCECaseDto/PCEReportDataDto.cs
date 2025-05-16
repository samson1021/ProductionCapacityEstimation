using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCECaseDto
{
    public class PCEReportDataDto
    {
        public PCECase PCECases { get; set; }
        public List<ProductionCapacity> Productions { get; set; }
        public List<PCEEvaluation>? PCEEvaluations { get; set; }
        public PCECaseSchedule PCECaseSchedule { get; set; }
        public List<ProductionLine>? ProductionLines { get; set; }

    }

    public class PCEReportDatalastDto
    {
        public PCECase? PCECases { get; set; }
        public List<ProductionCapacity>? Productions { get; set; }
        public List<PCEEvaluationDto>? PCEEvaluations { get; set; }
        public PCECaseSchedule? PCECaseSchedule { get; set; }
        public List<ProductionLineDto>? ProductionLines { get; set; }
    }

    public class PCEEvaluationDto
    {
        public Guid Id { get; set; }
        public Guid PCEId { get; set; }
        public Guid EvaluatorId { get; set; }
        public string? MachineName { get; set; }
        public string? CountryOfOrigin { get; set; }
        public bool HasInputOutputData { get; set; }
        public List<JustificationDto>? Justifications { get; set; }
        public ProductionLineType ProductionLineType { get; set; }
        public List<ProductionLineDto>? ProductionLines { get; set; }
        public string? TechnicalObsolescenceStatus { get; set; }
        public MachineFunctionalityStatus MachineFunctionalityStatus { get; set; }
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }
        public string? OtherMachineNonFunctionalityReason { get; set; }
        public string? FactorsAffectingProductionCapacity { get; set; }
        //public DateTimeRange? TimeConsumedToCheck { get; set; }
        public DateTimeRangeDto TimeConsumedToCheck { get; set; } = new DateTimeRangeDto();


        public string? InspectionPlace { get; set; }
        public DateOnly InspectionDate { get; set; }
        public string? SurveyRemark { get; set; }
        public string? Remark { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class DateTimeRangeDto
    {
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;

        // Optional: Add helper methods
        public bool IsValid => End > Start;
        public TimeSpan Duration => End - Start;
    }
    public class ProductionLineDto
    {
        public Guid Id { get; set; }
        public Guid PCEEvaluationId { get; set; }
        public Guid EvaluatorId { get; set; }
        public string? LineName { get; set; }
        public string? OutputType { get; set; }
        public OutputPhase? OutputPhase { get; set; }
        public bool IsBottleneck { get; set; }
        public decimal ActualCapacity { get; set; }
        public decimal? DesignCapacity { get; set; }
        public decimal? AttainableCapacity { get; set; }
        public MeasurementUnit ProductionUnit { get; set; }
        public ProductionMeasurement ProductionMeasurement { get; set; }
        public List<ProductionLineInputDto>? ProductionLineInputs { get; set; }
        public decimal? TotalInput { get; set; }
        public decimal? ConversionRatio { get; set; }
    }

    public class ProductionLineInputDto
    {
        public Guid Id { get; set; }
        public Guid ProductionLineId { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public MeasurementUnit Unit { get; set; }
    }

    public class JustificationDto
    {
        public Guid Id { get; set; }
        public Guid PCEEvaluationId { get; set; }
        public JustificationReason Reason { get; set; }
        public string? JustificationText { get; set; }
    }
}