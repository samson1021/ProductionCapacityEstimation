
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum Semiconductor
    {
        [Display(Name = "Semiconductor wafer fabrication")]
        SemiconductorWaferFabrication,

        [Display(Name = "Semiconductor packaging and testing")]
        SemiconductorPackagingAndTesting,

        [Display(Name = "Integrated circuit (IC) manufacturing")]
        IntegratedCircuitManufacturing,

        [Display(Name = "Semiconductor equipment manufacturing")]
        SemiconductorEquipmentManufacturing,

        [Display(Name = "Semiconductor materials manufacturing")]
        SemiconductorMaterialsManufacturing,

        [Display(Name = "Semiconductor component manufacturing")]
        SemiconductorComponentManufacturing,

        [Display(Name = "Semiconductor design and engineering services")]
        SemiconductorDesignAndEngineeringServices,

        [Display(Name = "Semiconductor research and development")]
        SemiconductorResearchAndDevelopment,

        [Display(Name = "Other")]
        Other
    }
}
