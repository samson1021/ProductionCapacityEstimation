using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class ProductionCapacityEstimation
    {
        [Key]
        public Guid Id { get; set; }
        // public Guid? CaseId { get; set; }
        // [ForeignKey("CaseId")]
        // public virtual Case? Case { get; set; }

        public string ProductionLineOrEquipmentName { get; set; }
        public string TypeOfOutput { get; set; }
        public string CountryOfOrigin { get; set; }
        public int? ShiftsPerDay { get; set; }

        public PhaseOfOutput? PhaseOfOutput { get; set; }
        public List<ShiftHour>? ShiftHours { get; set; } = new List<ShiftHour>();

        public int? WorkingDaysPerMonth { get; set; }
        public UnitOfProduction? UnitOfProduction { get; set; }
      
        public decimal? ProductionPerHour { get; set; }
        public int? EffectiveProductionHour { get; set; }
        public int? EffectiveProductionHourPerShift { get; set; }
        public ProductionHourType? EffectiveProductionHourType { get; set; }
       
        public ProductionMeasurement? ProductionMeasurement { get; set; }
        public decimal? EstimatedProductionCapacity { get; set; }
        public int? BottleneckProductionLineCapacity { get; set; }
        public int? OverallActualCurrentPlantCapacity { get; set; }

        public DateTime? TimeConsumedToCheckStart { get; set; }
        public DateTime? TimeConsumedToCheckEnd { get; set; }

        public TechnicalObsolescenceStatus? TechnicalObsolescenceStatus { get; set; }
        public decimal? DepreciationRateApplied { get; set; }
        public string Discrepancies { get; set; }

        public decimal? DesignProductionCapacity { get; set; }
        public decimal? AttainableProductionCapacity { get; set; }
        public decimal? ActualProductionCapacity { get; set; }

        public string PlaceOfInspection { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string FactorsAffectingProductionCapacity { get; set; }
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }
        public MachineFunctionalityReason? MachineFunctionalityReason { get; set; }
        public string OtherMachineFunctionalityReason { get; set; }
        public string SurveyRemark { get; set; }

        public virtual ICollection<UploadFile>? SupportingEvidences { get; set; } = new List<UploadFile>();
        public virtual ICollection<UploadFile>? ProductionProcessFlowDiagrams { get; set; } = new List<UploadFile>();
    
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public Guid CreatedBy { get; set; }

        // Calculated Fields
        [NotMapped]
        public decimal? PerShiftProduction => EffectiveProductionHourPerShift.HasValue ? EffectiveProductionHourPerShift.Value * ProductionPerHour : 0;

        [NotMapped]
        public decimal? PerDayProduction => ShiftsPerDay.HasValue ? ShiftsPerDay.Value * PerShiftProduction : 0;

        [NotMapped]
        public decimal? PerMonthProduction => WorkingDaysPerMonth.HasValue ? WorkingDaysPerMonth.Value * PerDayProduction : 0;

        [NotMapped]
        public decimal? PerYearProduction => PerMonthProduction * 12;
    
    }

    public class ShiftHour
    {
        [Key]
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
