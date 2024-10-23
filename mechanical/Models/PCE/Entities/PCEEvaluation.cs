using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Validators;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Entities
{
    public class PCEEvaluation
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PCEId")]
        public required Guid PCEId { get; set; }
        public Guid EvaluatorId { get; set; }
        
        public virtual ProductionCapacity PCE { get; set; }
        public virtual CreateUser Evaluator { get; set; }

        public string ProductionLineOrEquipmentName { get; set; }  
        public string OutputType { get; set; }
        public OutputPhase OutputPhase { get; set; }

        [Range(1, 5, ErrorMessage = "Shifts per day must be between 1 and 5.")]
        public int? ShiftsPerDay { get; set; }
        
        [ShiftHoursValidation]
        public virtual List<TimeInterval>? ShiftHours { get; set; } = new List<TimeInterval>();
        public int? WorkingDaysPerMonth { get; set; }
        public ProductionHourType? EffectiveProductionHourType { get; set; }
        public decimal? EffectiveProductionHour { get; set; }

        public ProductionUnit ProductionUnit { get; set; }
        public ProductionMeasurement ProductionMeasurement { get; set; }
        public string EstimatedProductionCapacity { get; set; }
        public string? BottleneckProductionLineCapacity { get; set; }
        public string OverallActualCurrentCapacity { get; set; }
        public virtual DateTimeRange TimeConsumedToCheck { get; set; }

        public string TechnicalObsolescenceStatus { get; set; }
        public decimal DepreciationRateApplied { get; set; }
        public string? Discrepancies { get; set; }

        public string ActualProductionCapacity { get; set; }
        public string? DesignProductionCapacity { get; set; }
        public string? AttainableProductionCapacity { get; set; }

        public string FactorsAffectingProductionCapacity { get; set; }
        public MachineFunctionalityStatus MachineFunctionalityStatus { get; set; }
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }
        public string? OtherMachineNonFunctionalityReason { get; set; }
        public string InspectionPlace { get; set; }
        public DateOnly InspectionDate { get; set; } 
        public string? SurveyRemark { get; set; }
        public string? Remark { get; set; } = string.Empty;
    
        public DateTime? CompletedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
    }
    public interface ITimeInterval
    {
        TimeSpan Start { get; set; }
        TimeSpan End { get; set; }
    }

    public class TimeInterval: ITimeInterval
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PCEEId")]
        public Guid PCEEId { get; set; }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

            
        public bool Contains(TimeSpan time)
        {
            return time >= Start && time <= End;
        }

        public TimeSpan Duration
        {
            get { return End - Start; }
        }
    }

    public class DateTimeRange
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PCEEId")]
        public Guid PCEEId { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= Start && dateTime <= End;
        }

        public TimeSpan Duration
        {
            get { return End - Start; }
        }
    }
}