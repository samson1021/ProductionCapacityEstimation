using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.ProductionCapacityEstimation;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class ProductionCapacityEstimation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? PCECaseId { get; set; } 
        // [ForeignKey("PCECaseId")]
        public virtual Case? PCECase { get; set; }

        public string ProductionLineOrEquipmentName { get; set; }
        public string? OutputType { get; set; }        
        public OutputPhase? OutputPhase { get; set; }

        public string? OriginCountry { get; set; }
        public ProductionUnit? ProductionUnit { get; set; }
        public int? WorkingDaysPerMonth { get; set; }
        public int? ShiftsPerDay { get; set; }
        public List<TimePeriod>? ShiftHours { get; set; } = new List<TimePeriod>();


        public ProductionMeasurement? ProductionMeasurement { get; set; }
        public decimal? EstimatedProductionCapacity { get; set; }
        public int? BottleneckProductionLineCapacity { get; set; }
        public int? OverallActualCurrentPlantCapacity { get; set; }
        // public TimePeriod? TimeConsumedToCheck { get; set; }
        public DateTimePeriod? TimeConsumedToCheck { get; set; } = new DateTimePeriod();

        public TechnicalObsolescenceStatus? TechnicalObsolescenceStatus { get; set; }
        public decimal? DepreciationRateApplied { get; set; }
        public string? Discrepancies { get; set; }
        public ProductionHourType? EffectiveProductionHourType { get; set; }
        public decimal? EffectiveProductionHour { get; set; }

        public decimal? DesignProductionCapacity { get; set; }
        public decimal? AttainableProductionCapacity { get; set; }
        public decimal? ActualProductionCapacity { get; set; }

        public string? FactorsAffectingProductionCapacity { get; set; }
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }
        public MachineFunctionalityReason? MachineFunctionalityReason { get; set; }
        public string? OtherMachineFunctionalityReason { get; set; }
        public string? InspectionPlace { get; set; }
        public DateOnly? InspectionDate { get; set; } //Schedule
        public string? SurveyRemark { get; set; }

        // public virtual ICollection<FileUpload>? SupportingEvidences { get; set; } = new List<FileUpload>();
        // public virtual ICollection<FileUpload>? ProductionProcessFlowDiagrams { get; set; } = new List<FileUpload>();
        public virtual ICollection<FileUpload> SupportingDocuments { get; set; } = new List<FileUpload>();

        public Status? Status { get; set; }
        public string? RejectionReason { get; set; }

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
