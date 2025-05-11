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
        NonFunctional
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

    public enum ProductionLineType
    {
        [Display(Name = "Mutually Exclusive")]
        MutuallyExclusive,

        [Display(Name = "Interdependent")]
        Interdependent,
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

    public enum MeasurementUnit
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

    
    public enum JustificationReason
    {
        [Display(Name = "The company refused to share input-output data as it considers confidential.")]
        Confidentiality,

        [Display(Name = "Legal or contractual obligations with partners prevent the disclosure of specific production details.")]
        LegalObligations,

        [Display(Name = "The company is not permitted for site visit team to access detailed production records.")]
        AccessRestricted,

        [Display(Name = "The site visit is too short to gather primary data in real-time.")]
        InsufficientTime,

        [Display(Name = "The plant is not operational during the visit.")]
        PlantNotOperational,

        [Display(Name = "Tracking input-output data is challenging due to highly complex or multi-stage processes.")]
        ComplexProcesses,

        [Display(Name = "Lack of cooperation by plant personnel to provide detailed information.")]
        LackOfCooperation,

        [Display(Name = "Measurement difficulties of some inputs or outputs that is hard to quantify accurately.")]
        MeasurementDifficulties,

        [Display(Name = "The company avoids scrutiny to hide inefficiencies, waste, or non-compliance with regulations by withholding data.")]
        AvoidScrutiny,

        [Display(Name = "The company assumes sharing input-output details may reveal competitive advantages or disadvantages.")]
        CompetitiveConcerns,

        [Display(Name = "The company avoids sharing data believing that the information will reflect poorly on its operations.")]
        FearOfNegativeReflection,

        [Display(Name = "The purpose of the site visit is not well-explained by CRM, hence, the company is not willing to provide the necessary information.")]
        PurposeNotExplained,

        [Display(Name = "Others, please explain (comment box).")]
        Others
    }

}
