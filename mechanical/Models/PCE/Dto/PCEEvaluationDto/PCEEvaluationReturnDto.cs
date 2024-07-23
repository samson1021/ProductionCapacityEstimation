using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;


namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEEvaluationReturnDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Evaluated By")]
        public Guid EvaluatorId { get; set; }

        public required Guid PCEId { get; set; }
        public virtual ProductionCapacity PCE { get; set; }

        [Display(Name = "Production Line/Equipment Name")]
        public string ProductionLineOrEquipmentName { get; set; }

<<<<<<< HEAD
        [Display(Name = "Type of Output")]
        public string OutputType { get; set; }
        
        [Display(Name = "Phase of Output")]
        public OutputPhase OutputPhase { get; set; }

=======
        [Display(Name = "Phase of Output")]
        public OutputPhase? OutputPhase { get; set; }

        [Display(Name = "Country of Origin")]
        public string? OriginCountry { get; set; }

>>>>>>> Abdu
        [Display(Name = "Shifts Per Day")]
        public int? ShiftsPerDay { get; set; }

        [Display(Name = "Shift Hours")]
        public List<TimeIntervalDto>? ShiftHours { get; set; } = new List<TimeIntervalDto>();

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
        public string OverallActualCurrentPlantCapacity { get; set; }

        [Display(Name = "Time Consumed to Check")]
        public DateTimeRangeDto TimeConsumedToCheck { get; set; }

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

        [Display(Name = "Supporting Evidences")]
        public ICollection<ReturnFileDto> SupportingEvidences { get; set; }

        [Display(Name = "Production Process Flow Diagrams")]  
        public ICollection<ReturnFileDto> ProductionProcessFlowDiagrams { get; set; }  

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } 

    }
}
