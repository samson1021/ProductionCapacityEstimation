using System;
using System.Collections.Generic;

using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.ProductionCapacityEstimation;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class ProductionCapacityEstimationPostDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; } 

        public string ProductionLineOrEquipmentName { get; set; }
        public string TypeOfOutput { get; set; }
        public string CountryOfOrigin { get; set; }
        public int? ShiftsPerDay { get; set; }
        public int? WorkingDaysPerMonth { get; set; }
        public UnitOfProduction? UnitOfProduction { get; set; }
        public decimal? ProductionPerHour { get; set; }
        public int? EffectiveProductionHourPerShift { get; set; }
        public decimal? DesignProductionCapacity { get; set; }
        public decimal? AttainableProductionCapacity { get; set; }
        public decimal? ActualProductionCapacity { get; set; }
        public string FactorsAffectingProductionCapacity { get; set; }
        public MachineFunctionalityStatus? MachineFunctionalityStatus { get; set; }
        public MachineFunctionalityReason? MachineFunctionalityReason { get; set; }
        public string OtherMachineFunctionalityReason { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } 

        public PhaseOfOutput? PhaseOfOutput { get; set; }
        public List<ShiftHourDto>? ShiftHours { get; set; } = new List<ShiftHourDto>();
        public int? EffectiveProductionHour { get; set; }
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
        public string PlaceOfInspection { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string SurveyRemark { get; set; } = string.Empty;

        public decimal? PerShiftProduction { get; set; }
        public decimal? PerDayProduction { get; set; }
        public decimal? PerMonthProduction { get; set; }
        public decimal? PerYearProduction { get; set; }

        public ICollection<IFormFile>? SupportingEvidences { get; set; }
        public ICollection<IFormFile>? ProductionProcessFlowDiagrams { get; set; }     
        // public List<ReturnFileDto>? SupportingEvidences { get; set; } = new List<ReturnFileDto>();
        // public List<ReturnFileDto>? ProductionProcessFlowDiagrams { get; set; } = new List<ReturnFileDto>();
        // public List<CreateFileDto> SupportingEvidences { get; set; } = new List<CreateFileDto>();
        // // public List<CreateFileDto> ProductionProcessFlowDiagrams { get; set; } = new List<CreateFileDto>();  
    
    }
}