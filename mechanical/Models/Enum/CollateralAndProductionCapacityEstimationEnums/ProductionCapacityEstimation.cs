using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.ProductionCapacityEstimation
{
    public enum PhaseOfOutput
    {
        [Display(Name = "Finished Product")]
        FinishedProduct,

        [Display(Name = "Raw Material for Next Line")]
        RawMaterialForNextLine
    }

    public enum TechnicalObsolescenceStatus
    {
        [Display(Name = "Obsolete")]
        Obsolete,

        [Display(Name = "Good")]
        Good
    }

    public enum MachineFunctionalityStatus
    {
        [Display(Name = "Functional at Time of Visit")]
        FunctionalAtTimeOfVisit,

        [Display(Name = "Not Functional at Time of Visit")]
        NotFunctionalAtTimeOfVisit
    }

    public enum MachineFunctionalityReason
    {
        [Display(Name = "Power Shutdown")]
        PowerShutdown,

        [Display(Name = "Raw Materials Shortage")]
        RawMaterialsShortage,

        [Display(Name = "Scheduled Maintenance")]
        ScheduledMaintenance,

        [Display(Name = "Others")]
        Others
    }

    public enum Status
    {
        [Display(Name = "New")]
        New,

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Rejected")]
        Rejected,

        [Display(Name = "Validated")]
        Validated,

        [Display(Name = "Committed")]
        Committed,

        [Display(Name = "Approved")]
        Approved,
        
        [Display(Name = "Terminated")]
        Terminated

    }

    public enum ProductionHourType
    {
        [Display(Name = "Per Shift")]
        PerShift,

        [Display(Name = "Per Day")]
        PerDay
    }

    public enum ProductionMeasurement
    {
        [Display(Name = "Per Hour")]
        PerHour,

        [Display(Name = "Per Shift")]
        PerShift,

        [Display(Name = "Per Day")]
        PerDay,

        [Display(Name = "Per Month")]
        PerMonth,

        [Display(Name = "Per Year")]
        PerYear
    }

    public enum UnitOfProduction
    {
        [Display(Name = "Number")]
        Number,

        [Display(Name = "Kilogram")]
        Kilogram,

        [Display(Name = "Pound")]
        Pound,

        [Display(Name = "Quintal")]
        Quintal,

        [Display(Name = "Ton")]
        Ton,

        [Display(Name = "Pairs")]
        Pairs,

        [Display(Name = "Pieces")]
        Pcs,

        [Display(Name = "Liter")]
        Liter,

        [Display(Name = "Gallon")]
        Gallon,

        [Display(Name = "Meter")]
        Meter,

        [Display(Name = "Square Meter")]
        M2,

        [Display(Name = "Cubic Meter")]
        M3,

        [Display(Name = "Cubic Feet")]
        Ft3,

        [Display(Name = "Hectoliter")]
        Hl,

        [Display(Name = "Thousand Cubic Feet")]
        Mcf,

        [Display(Name = "Carat")]
        Ct,

        [Display(Name = "Ton Refrigeration")]
        TR,

        [Display(Name = "Thousand Units")]
        KU,

        [Display(Name = "Sets")]
        Sets,

        [Display(Name = "Units")]
        Units,

        [Display(Name = "Others")]
        Others
    }
}
