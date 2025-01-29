
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum EdibleOilSubsector
    {
        [Display(Name = "Vegetable oil refining and processing")]
        VegetableOilRefiningAndProcessing,

        [Display(Name = "Olive oil production")]
        OliveOilProduction,

        [Display(Name = "Seed oil production")]
        SeedOilProduction,

        [Display(Name = "Nut oil production")]
        NutOilProduction,

        [Display(Name = "Specialty oil production (e.g., avocado oil)")]
        SpecialtyOilProduction,

        [Display(Name = "Edible oil packaging")]
        EdibleOilPackaging,

        [Display(Name = "Oil extraction equipment manufacturing")]
        OilExtractionEquipmentManufacturing,

        [Display(Name = "Oil filtration and purification equipment manufacturing")]
        OilFiltrationAndPurificationEquipmentManufacturing,

        [Display(Name = "Margarine and Shortening Production")]
        MargarineAndShorteningProduction,

        [Display(Name = "Other")]
        Other
    }
}
