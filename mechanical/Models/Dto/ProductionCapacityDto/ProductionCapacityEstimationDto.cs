using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums;

namespace mechanical.Models.Dto.ProductionCapacityDto
{
    public class ProductionCapacityEstimationDto
    {
        public Guid Id { get; set; }
        public Guid? CaseId { get; set; }

        public string ProductionLineOrEquipmentName { get; set; }
        public string TypeOfOutput { get; set; }
        public string CountryOfOrigin { get; set; }
        public int? ShiftsPerDay { get; set; }

        public PhaseOfOutput? PhaseOfOutput { get; set; }
        public List<ShiftHourDto>? ShiftHours { get; set; } = new List<ShiftHourDto>();

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

        public decimal? PerShiftProduction { get; set; }
        public decimal? PerDayProduction { get; set; }
        public decimal? PerMonthProduction { get; set; }
        public decimal? PerYearProduction { get; set; }

        public string Status { get; set; }
        public string RejectionReason { get; set; }

        public List<ReturnFileDto>? SupportingEvidences { get; set; } = new List<ReturnFileDto>();
        public List<ReturnFileDto>? ProductionProcessFlowDiagrams { get; set; } = new List<ReturnFileDto>();
    
        // public List<UploadFileDto> SupportingEvidences { get; set; } = new List<UploadFileDto>();
        // // public List<UploadFileDto> ProductionProcessFlowDiagrams { get; set; } = new List<UploadFileDto>();  
    }

    public class ShiftHourDto
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

}


