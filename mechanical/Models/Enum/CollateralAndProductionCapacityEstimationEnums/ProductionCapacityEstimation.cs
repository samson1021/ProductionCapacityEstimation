using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums
{
    public enum PhaseOfOutput
    {
        FinishedProduct,
        RawMaterialForNextLine
    }

    public enum TechnicalObsolescenceStatus
    {
        Obsolete,
        Good
    }

    public enum MachineFunctionalityStatus
    {
        FunctionalAtTimeOfVisit,
        NotFunctionalAtTimeOfVisit
    }

    public enum MachineFunctionalityReason
    {
        PowerShutdown, 
        RawMaterialsShortage, 
        ScheduledMaintenance, 
        Others
    }

    public enum ProductionHourType
    {
        PerShift,
        PerDay
    }

    public enum ProductionMeasurement
    {
        PerHour,
        PerShift,
        PerDay,
        PerMonth,
        PerYear
    }
    
    public enum UnitOfProduction
    {
        Number, 
        Kilogram, 
        Pound, 
        Quintal, 
        Ton, 
        Pairs, 
        Pcs, 
        Liter, 
        Gallon, 
        Meter, 
        M2, 
        M3, 
        Ft3, 
        Hl, 
        Mcf, 
        Ct, 
        TR, KU, 
        Sets, 
        Units, 
        Others
    }
}