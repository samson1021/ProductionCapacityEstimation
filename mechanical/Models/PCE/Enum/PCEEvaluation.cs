using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.PCEEvaluation
{
    public enum OutputPhase
    {
        [Display(Name = "Finished Product")]
        FinishedProduct,
        
        [Display(Name = "Raw Material For Next Line")]
        RawMaterialForNextLine
    }

    public enum MachineFunctionalityStatus
    {
        [Display(Name = "Functional at Time of Visit")]
        Functional,

        [Display(Name = "Not Functional at Time of Visit")]
        NotFunctional
    }

    public enum MachineNonFunctionalityReason
    {
        [Display(Name = "Power Shutdown")]
        PowerShutdown,

        [Display(Name = "Raw Materials Shortage")]
        RawMaterialsShortage,

        [Display(Name = "Scheduled Maintenance")]
        ScheduledMaintenance,

        [Display(Name = "Other")]
        Other
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

    public enum ProductionUnit
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

        [Display(Name = "Other")]
        Other
    }
}
