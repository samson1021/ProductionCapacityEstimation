
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum MetalRecycling
    {
        [Display(Name = "Scrap metal collection and processing")]
        ScrapMetalCollectionAndProcessing,

        [Display(Name = "Metal shredding and sorting")]
        MetalShreddingAndSorting,

        [Display(Name = "Ferrous metal recycling")]
        FerrousMetalRecycling,

        [Display(Name = "Non-ferrous metal recycling")]
        NonFerrousMetalRecycling,

        [Display(Name = "Metal alloy recycling")]
        MetalAlloyRecycling,

        [Display(Name = "Metal smelting and refining services")]
        MetalSmeltingAndRefiningServices,

        [Display(Name = "Metal waste management and disposal")]
        MetalWasteManagementAndDisposal,

        [Display(Name = "Metal recycling equipment manufacturing")]
        MetalRecyclingEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
