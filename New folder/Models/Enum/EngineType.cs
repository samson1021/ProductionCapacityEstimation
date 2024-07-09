using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum EngineType
    {
        [Display(Name = "Diesel Engine")]
        DieselEngine,

        [Display(Name = "Gasoline Engine")]
        GasolineEngine,

        [Display(Name = "Electric Motor")]
        ElectricMotor,

        [Display(Name = "Hybrid with Diesel Engine")]
        HybridDieselEngine,

        [Display(Name = "Hybrid with Gasoline Engine")]
        HybridGasolineEngine,

        [Display(Name = "Other")]
        Other,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
