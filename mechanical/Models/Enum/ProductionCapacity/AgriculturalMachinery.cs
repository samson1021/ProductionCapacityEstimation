
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum AgriculturalMachinery
    {
        [Display(Name = "Tractor manufacturing")]
        TractorManufacturing,

        [Display(Name = "Combine harvester manufacturing")]
        CombineHarvesterManufacturing,

        [Display(Name = "Irrigation equipment manufacturing")]
        IrrigationEquipmentManufacturing,

        [Display(Name = "Seed processing equipment manufacturing")]
        SeedProcessingEquipmentManufacturing,

        [Display(Name = "Livestock equipment manufacturing")]
        LivestockEquipmentManufacturing,

        [Display(Name = "Farm implement manufacturing")]
        FarmImplementManufacturing,

        [Display(Name = "Agricultural sprayer manufacturing")]
        AgriculturalSprayerManufacturing,

        [Display(Name = "Precision agriculture technology manufacturing")]
        PrecisionAgricultureTechnologyManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
