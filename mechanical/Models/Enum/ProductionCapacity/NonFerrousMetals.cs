
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum NonFerrousMetals
    {
        [Display(Name = "Aluminum production and processing")]
        AluminumProductionAndProcessing,

        [Display(Name = "Copper production and processing")]
        CopperProductionAndProcessing,

        [Display(Name = "Zinc production and processing")]
        ZincProductionAndProcessing,

        [Display(Name = "Lead production and processing")]
        LeadProductionAndProcessing,

        [Display(Name = "Nickel production and processing")]
        NickelProductionAndProcessing,

        [Display(Name = "Tin production and processing")]
        TinProductionAndProcessing,

        [Display(Name = "Precious metal production and processing (excluding gold)")]
        PreciousMetalProductionAndProcessingExcludingGold,

        [Display(Name = "Non-ferrous metal recycling and scrap processing")]
        NonFerrousMetalRecyclingAndScrapProcessing,

        [Display(Name = "Other")]
        Other
    }
}
