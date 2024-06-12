
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum SpiceAndSeasoning
    {
        [Display(Name = "Spice blending and packaging")]
        SpiceBlendingAndPackaging,

        [Display(Name = "Spice processing and grinding")]
        SpiceProcessingAndGrinding,

        [Display(Name = "Spice extract manufacturing")]
        SpiceExtractManufacturing,

        [Display(Name = "Seasoning mix manufacturing")]
        SeasoningMixManufacturing,

        [Display(Name = "Herb and spice infusion manufacturing")]
        HerbAndSpiceInfusionManufacturing,

        [Display(Name = "Spice and seasoning distribution and wholesaling")]
        SpiceAndSeasoningDistributionAndWholesaling,

        [Display(Name = "Spice and seasoning retail packaging")]
        SpiceAndSeasoningRetailPackaging,

        [Display(Name = "Spice and seasoning export and distribution")]
        SpiceAndSeasoningExportAndDistribution,

        [Display(Name = "Other")]
        Other
    }
}
