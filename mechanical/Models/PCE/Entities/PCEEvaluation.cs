using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Entities
{
    public class PCEEvaluation
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PCEId")]
        public Guid PCEId { get; set; } 
        // public required Guid PCEId { get; set; }
        public Guid EvaluatorID { get; set; }
        
        public virtual ProductionCapacity? PCE { get; set; }
        public virtual CreateUser? Evaluator { get; set; }
        public string ProductionLineOrEquipmentName { get; set; }   
        public OutputPhase? OutputPhase { get; set; }

        public string? OriginCountry { get; set; }
        public ProductionUnit? ProductionUnit { get; set; }
        public int? WorkingDaysPerMonth { get; set; }
        public int? ShiftsPerDay { get; set; }
        public List<TimePeriod>? ShiftHours { get; set; } = new List<TimePeriod>();

        public ProductionMeasurement? ProductionMeasurement { get; set; }
        public string? EstimatedProductionCapacity { get; set; }
        public string? BottleneckProductionLineCapacity { get; set; }
        public string? OverallActualCurrentPlantCapacity { get; set; }
        public DateTimePeriod? TimeConsumedToCheck { get; set; } = new DateTimePeriod();

        public string? TechnicalObsolescenceStatus { get; set; }
        public decimal? DepreciationRateApplied { get; set; }
        public string? Discrepancies { get; set; }
        public ProductionHourType? EffectiveProductionHourType { get; set; }
        public decimal? EffectiveProductionHour { get; set; }

        public string? DesignProductionCapacity { get; set; }
        public string? AttainableProductionCapacity { get; set; }
        public string? ActualProductionCapacity { get; set; }

        public string? FactorsAffectingProductionCapacity { get; set; }
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }
        public string? OtherMachineNonFunctionalityReason { get; set; }
        public string? InspectionPlace { get; set; }
        public DateOnly? InspectionDate { get; set; } //Schedule
        public string? SurveyRemark { get; set; }
    
        public virtual ICollection<UploadFile> SupportingDocuments { get; set; } = new List<UploadFile>();

        public Status? Status { get; set; }
        // public string? ReturnReason { get; set; }

        public string? Remark { get; set; } = string.Empty;
        
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TimePeriod
    {
        [Key]
        public int Id { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }

    public class DatePeriod
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }

    public class DateTimePeriod
    {
        [Key]
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}