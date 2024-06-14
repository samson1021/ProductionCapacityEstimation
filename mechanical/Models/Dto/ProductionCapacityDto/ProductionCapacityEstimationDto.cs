using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.ProductionCapacityEstimation;


namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class ProductionCapacityEstimationDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Case Id")]
        public Guid? CaseId { get; set; } 

        [Display(Name = "Production Line/Equipment Name")]
        public string ProductionLineOrEquipmentName { get; set; }

        [Display(Name = "Type of Output")]
        public string? TypeOfOutput { get; set; }

        [Display(Name = "Country of Origin")]
        public string? CountryOfOrigin { get; set; }

        [Display(Name = "Shifts Per Day")]
        public int? ShiftsPerDay { get; set; }

        [Display(Name = "Working Days Per Month")]
        public int? WorkingDaysPerMonth { get; set; }

        [Display(Name = "Unit of Production")]
        public UnitOfProduction? UnitOfProduction { get; set; }

        [Display(Name = "Production Per Hour")]
        public decimal? ProductionPerHour { get; set; }

        [Display(Name = "Effective Production Hour Per Shift")]
        public int? EffectiveProductionHourPerShift { get; set; }

        [Display(Name = "Design Production Capacity")]
        public decimal? DesignProductionCapacity { get; set; }

        [Display(Name = "Attainable Production Capacity")]
        public decimal? AttainableProductionCapacity { get; set; }

        [Display(Name = "Actual Production Capacity")]
        public decimal? ActualProductionCapacity { get; set; }

        [Display(Name = "Factors Affecting Production Capacity")]
        public string? FactorsAffectingProductionCapacity { get; set; }

        [Display(Name = "Machine Functionality Status")]
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }

        [Display(Name = "Machine Functionality Reason")]
        public MachineFunctionalityReason? MachineFunctionalityReason { get; set; }

        [Display(Name = "Other Machine Functionality Reason")]
        public string? OtherMachineFunctionalityReason { get; set; }

        [Display(Name = "Status")]
        public Status? Status { get; set; }

        [Display(Name = "Rejection Reason")]
        public string? RejectionReason { get; set; }

        [Display(Name = "Created By")]
        public Guid CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } 

        [Display(Name = "Phase of Output")]
        public PhaseOfOutput? PhaseOfOutput { get; set; }

        [Display(Name = "Shift Hours")]
        public List<ShiftHourDto>? ShiftHours { get; set; } = new List<ShiftHourDto>();

        [Display(Name = "Effective Production Hour")]
        public int? EffectiveProductionHour { get; set; }

        [Display(Name = "Effective Production Hour Type")]
        public ProductionHourType? EffectiveProductionHourType { get; set; }

        [Display(Name = "Production Measurement")]
        public ProductionMeasurement? ProductionMeasurement { get; set; }

        [Display(Name = "Estimated Production Capacity")]
        public decimal? EstimatedProductionCapacity { get; set; }

        [Display(Name = "Bottleneck Production Line Capacity")]
        public int? BottleneckProductionLineCapacity { get; set; }

        [Display(Name = "Overall Actual Current Plant Capacity")]
        public int? OverallActualCurrentPlantCapacity { get; set; }

        [Display(Name = "Time Consumed to Check Start")]
        public DateTime? TimeConsumedToCheckStart { get; set; }

        [Display(Name = "Time Consumed to Check End")]
        public DateTime? TimeConsumedToCheckEnd { get; set; }

        [Display(Name = "Technical Obsolescence Status")]
        public TechnicalObsolescenceStatus? TechnicalObsolescenceStatus { get; set; }

        [Display(Name = "Depreciation Rate Applied")]
        public decimal? DepreciationRateApplied { get; set; }

        [Display(Name = "Discrepancies")]
        public string? Discrepancies { get; set; }

        [Display(Name = "Place of Inspection")]
        public string? PlaceOfInspection { get; set; }

        [Display(Name = "Inspection Date")]
        public DateTime? InspectionDate { get; set; }

        [Display(Name = "Survey Remark")]
        public string? SurveyRemark { get; set; }

    
        [Display(Name = "Per Shift Production")]
        public decimal? PerShiftProduction { get; set; }

        [Display(Name = "Per Day Production")]
        public decimal? PerDayProduction { get; set; }

        [Display(Name = "Per Month Production")]
        public decimal? PerMonthProduction { get; set; }

        [Display(Name = "Per Year Production")]
        public decimal? PerYearProduction { get; set; }


        [Display(Name = "Supporting Evidences")]
        public ICollection<UploadFile>? SupportingEvidences { get; set; }

        [Display(Name = "Production Process Flow Diagrams")]
        public ICollection<UploadFile>? ProductionProcessFlowDiagrams { get; set; }  
     
        // public List<ReturnFileDto>? SupportingEvidences { get; set; } = new List<ReturnFileDto>();
        // public List<ReturnFileDto>? ProductionProcessFlowDiagrams { get; set; } = new List<ReturnFileDto>();
        // public List<CreateFileDto> SupportingEvidences { get; set; } = new List<CreateFileDto>();
        // // public List<CreateFileDto> ProductionProcessFlowDiagrams { get; set; } = new List<CreateFileDto>();  
          
    }

    public class ShiftHourDto
    {
        public int Id { get; set; }

        [Display(Name = "Start Time")]
        public DateTime Start { get; set; }

        [Display(Name = "End Time")]
        public DateTime End { get; set; }
    }
}
