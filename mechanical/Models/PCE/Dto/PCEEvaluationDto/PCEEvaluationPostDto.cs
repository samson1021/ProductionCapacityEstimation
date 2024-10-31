using System.ComponentModel.DataAnnotations;

using mechanical.Utils;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEEvaluationPostDto
    {
        public required Guid PCEId { get; set; } 

        [Display(Name = "Production Line/Equipment Name")]
        public string ProductionLineOrEquipmentName { get; set; }

        [Display(Name = "Type of Output")]
        public string OutputType { get; set; }

        [Display(Name = "Phase of Output")]
        public OutputPhase OutputPhase { get; set; }

        [Display(Name = "Country of Origin")]
        public string? OriginCountry { get; set; }

        [Display(Name = "Shifts Per Day")]
        [Range(1, 5, ErrorMessage = "Shifts per day must be between 1 and 5.")]
        public int? ShiftsPerDay { get; set; }

        [ShiftHoursValidation]
        [Display(Name = "Shift Hours")]
        public virtual List<TimeIntervalPostDto>? ShiftHours { get; set; } = new List<TimeIntervalPostDto>();

        [Display(Name = "Working Days Per Month")]
        public int? WorkingDaysPerMonth { get; set; }

        [Display(Name = "Effective Production Hour Type")]
        public ProductionHourType? EffectiveProductionHourType { get; set; }

        [Display(Name = "Effective Production Hour")]
        public decimal? EffectiveProductionHour { get; set; }

        [Display(Name = "Unit of Production")]
        public ProductionUnit ProductionUnit { get; set; }

        [Display(Name = "Production Measurement")]
        public ProductionMeasurement ProductionMeasurement { get; set; }
        
        [Display(Name = "Estimated Production Capacity")]
        public string EstimatedProductionCapacity { get; set; }

        [Display(Name = "Bottleneck Production Line Capacity")]
        public string? BottleneckProductionLineCapacity { get; set; }

        [Display(Name = "Overall Actual Current Capacity")]
        public string OverallActualCurrentCapacity { get; set; }

        [Display(Name = "Time Consumed to Check")]
        public virtual DateTimeRangePostDto TimeConsumedToCheck { get; set; }

        [Display(Name = "Technical Obsolescence Status")]
        public string TechnicalObsolescenceStatus { get; set; }

        [Display(Name = "Depreciation Rate Applied")]
        public decimal DepreciationRateApplied { get; set; }

        [Display(Name = "Discrepancies")]
        public string? Discrepancies { get; set; }

        [Display(Name = "Actual Production Capacity")]
        public string ActualProductionCapacity { get; set; }

        [Display(Name = "Design Production Capacity")]
        public string? DesignProductionCapacity { get; set; }

        [Display(Name = "Attainable Production Capacity")]
        public string? AttainableProductionCapacity { get; set; }

        [Display(Name = "Factors Affecting Production Capacity")]
        public string FactorsAffectingProductionCapacity { get; set; }

        [Display(Name = "Machine Functionality Status")]
        public MachineFunctionalityStatus MachineFunctionalityStatus { get; set; }

        [Display(Name = "Non-Functionality Reason")]
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }

        [Display(Name = "Other Non-Functionality Reason")]
        public string? OtherMachineNonFunctionalityReason { get; set; }

        [Display(Name = "Place of Inspection")]
        public string InspectionPlace { get; set; }

        [Display(Name = "Inspection Date")]
        public DateOnly InspectionDate { get; set; }

        [Display(Name = "Survey Remark")]
        public string? SurveyRemark { get; set; }
        
        [Display(Name = "Remark")]
        public string? Remark { get; set; } = string.Empty;

        [Display(Name = "Supporting Evidences")]
        public ICollection<IFormFile> SupportingEvidences { get; set; }

        [Display(Name = "Production Process Flow Diagrams")] 
        public ICollection<IFormFile> ProductionProcessFlowDiagrams { get; set; } 
    }

    public class TimeIntervalPostDto: ITimeInterval
    {
        public Guid PCEEId { get; set; } 

        [Display(Name = "Start Time")]
        public TimeSpan Start { get; set; }

        [Display(Name = "End Time")]
        public TimeSpan End { get; set; }
    }

    public class DateTimeRangePostDto
    {
        public Guid PCEEId { get; set; }

        [Display(Name = "Start DateTime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Display(Name = "End DateTime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
    }
}