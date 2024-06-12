
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum SteelRecyclingAndScrapProcessing
    {
        [Display(Name = "Scrap metal collection and processing")]
        ScrapMetalCollectionAndProcessing,

        [Display(Name = "Steel scrap sorting and grading")]
        SteelScrapSortingAndGrading,

        [Display(Name = "Metal shredding and shearing")]
        MetalShreddingAndShearing,

        [Display(Name = "Ferrous metal recycling")]
        FerrousMetalRecycling,

        [Display(Name = "Non-ferrous metal recycling")]
        NonFerrousMetalRecycling,

        [Display(Name = "Metal smelting and refining")]
        MetalSmeltingAndRefining,

        [Display(Name = "Steel scrap trading and export")]
        SteelScrapTradingAndExport,

        [Display(Name = "Metal waste management and disposal")]
        MetalWasteManagementAndDisposal,

        [Display(Name = "Other")]
        Other
    }
}
