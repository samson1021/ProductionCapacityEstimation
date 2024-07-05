using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;


namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public class PCEEvaluationPostDto
    {
        public Guid Id { get; set; }

        // [Display(Name = "Evaluated By")]
        // public Guid EvaluatorID { get; set; }

        // public required Guid? PCEId { get; set; } 
        public Guid? PCEId { get; set; } 

        [Display(Name = "Production Line/Equipment Name")]
        public string ProductionLineOrEquipmentName { get; set; }


        [Display(Name = "Phase of Output")]
        public OutputPhase? OutputPhase { get; set; }

        [Display(Name = "Country of Origin")]
        public string? OriginCountry { get; set; }

        [Display(Name = "Shifts Per Day")]
        public int? ShiftsPerDay { get; set; }

        [Display(Name = "Shift Hours")]
        public List<TimePeriodDto>? ShiftHours { get; set; } = new List<TimePeriodDto>();

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
        public DateTimePeriodDto? TimeConsumedToCheck { get; set; }

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

        [Display(Name = "Machine Functionality Reason")]
        public MachineFunctionalityReason? MachineFunctionalityReason { get; set; }

        [Display(Name = "Other Machine Functionality Reason")]
        public string? OtherMachineFunctionalityReason { get; set; }

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
     
        [Display(Name = "Status")]
        public Status? Status { get; set; }

        [Display(Name = "Remark")]
        public string? Remark { get; set; }

        [Display(Name = "Rejection Reason")]
        public string? RejectionReason { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } 
    }

    public class TimePeriodDto
    {
        public int Id { get; set; }

        [Display(Name = "Start Time")]
        public TimeOnly Start { get; set; }

        [Display(Name = "End Time")]
        public TimeOnly End { get; set; }
    }

    public class DatePeriodDto
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly Start { get; set; }

        [Display(Name = "End Date")]
        public DateOnly End { get; set; }
    }

    public class DateTimePeriodDto
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
