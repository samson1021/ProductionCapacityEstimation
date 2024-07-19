using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Enum.PCEEvaluation;


namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEEvaluationPostDto
    {
        public required Guid PCEId { get; set; } 

        [Display(Name = "Production Line/Equipment Name")]
        public string ProductionLineOrEquipmentName { get; set; }

<<<<<<< HEAD
        [Display(Name = "Type of Output")]
        public string? OutputType { get; set; }
        
        [Display(Name = "Phase of Output")]
        public OutputPhase? OutputPhase { get; set; }

=======
        [Display(Name = "Phase of Output")]
        public OutputPhase? OutputPhase { get; set; }

        [Display(Name = "Country of Origin")]
        public string? OriginCountry { get; set; }

>>>>>>> Abdu
        [Display(Name = "Shifts Per Day")]
        public int? ShiftsPerDay { get; set; }

        [Display(Name = "Shift Hours")]
<<<<<<< HEAD
        public List<TimeRangeDto>? ShiftHours { get; set; }
=======
        public List<TimePeriodDto>? ShiftHours { get; set; } = new List<TimePeriodDto>();
>>>>>>> Abdu

        [Display(Name = "Working Days Per Month")]
        public int? WorkingDaysPerMonth { get; set; }

        [Display(Name = "Unit of Production")]
        public ProductionUnit? ProductionUnit { get; set; }

        [Display(Name = "Production Measurement")]
        public ProductionMeasurement? ProductionMeasurement { get; set; }
        
        [Display(Name = "Estimated Production Capacity")]
        public string? EstimatedProductionCapacity { get; set; }

        [Display(Name = "Bottleneck Production Line Capacity")]
        public string? BottleneckProductionLineCapacity { get; set; }

        [Display(Name = "Overall Actual Current Capacity")]
        public string? OverallActualCurrentPlantCapacity { get; set; }

        [Display(Name = "Time Consumed to Check")]
<<<<<<< HEAD
        public DateTimeRangeDto? TimeConsumedToCheck { get; set; }
=======
        public DateTimePeriodDto? TimeConsumedToCheck { get; set; }
>>>>>>> Abdu

        [Display(Name = "Technical Obsolescence Status")]
        public string? TechnicalObsolescenceStatus { get; set; }

        [Display(Name = "Depreciation Rate Applied")]
        public decimal? DepreciationRateApplied { get; set; }

        [Display(Name = "Discrepancies")]
        public string? Discrepancies { get; set; }

        [Display(Name = "Effective Production Hour Type")]
        public ProductionHourType? EffectiveProductionHourType { get; set; }

        [Display(Name = "Effective Production Hour")]
        public decimal? EffectiveProductionHour { get; set; }

        [Display(Name = "Design Production Capacity")]
        public string? DesignProductionCapacity { get; set; }

        [Display(Name = "Attainable Production Capacity")]
        public string? AttainableProductionCapacity { get; set; }

        [Display(Name = "Actual Production Capacity")]
        public string? ActualProductionCapacity { get; set; }

        [Display(Name = "Factors Affecting Production Capacity")]
        public string? FactorsAffectingProductionCapacity { get; set; }

        [Display(Name = "Machine Functionality Status")]
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }

        [Display(Name = "Non-Functionality Reason")]
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }

        [Display(Name = "Other Non-Functionality Reason")]
        public string? OtherMachineNonFunctionalityReason { get; set; }

        [Display(Name = "Place of Inspection")]
        public string? InspectionPlace { get; set; }

        [Display(Name = "Inspection Date")]
        public DateOnly? InspectionDate { get; set; }

        [Display(Name = "Survey Remark")]
        public string? SurveyRemark { get; set; }

        [Display(Name = "Supporting Evidences")]
        public ICollection<IFormFile>? SupportingEvidences { get; set; }

        [Display(Name = "Production Process Flow Diagrams")] 
        public ICollection<IFormFile>? ProductionProcessFlowDiagrams { get; set; }  
<<<<<<< HEAD

        [Display(Name = "Remark")]
        public string? Remark { get; set; }
    }

    public class TimeRangeDto
=======
     
        [Display(Name = "Status")]
        public Status? Status { get; set; }

        [Display(Name = "Remark")]
        public string? Remark { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } 
    }

    public class TimePeriodDto
>>>>>>> Abdu
    {
        public int Id { get; set; }

        [Display(Name = "Start Time")]
        public TimeOnly Start { get; set; }

        [Display(Name = "End Time")]
        public TimeOnly End { get; set; }
    }

<<<<<<< HEAD
    // public class DateRangeDto
    // {
    //     public int Id { get; set; }

    //     [Display(Name = "Start Date")]
    //     public DateOnly Start { get; set; }

    //     [Display(Name = "End Date")]
    //     public DateOnly End { get; set; }
    // }

    public class DateTimeRangeDto
=======
    public class DatePeriodDto
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly Start { get; set; }

        [Display(Name = "End Date")]
        public DateOnly End { get; set; }
    }

    public class DateTimePeriodDto
>>>>>>> Abdu
    {
        public int Id { get; set; }

        [Display(Name = "Start Date & Time")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Display(Name = "End Date & Time")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
    }
}
