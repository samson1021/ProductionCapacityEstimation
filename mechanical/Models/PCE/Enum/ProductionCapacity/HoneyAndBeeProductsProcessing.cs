
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum HoneyAndBeeProductsProcessing
    {
        [Display(Name = "Honey extraction and processing")]
        HoneyExtractionAndProcessing,

        [Display(Name = "Beeswax production and processing")]
        BeeswaxProductionAndProcessing,

        [Display(Name = "Royal jelly production and processing")]
        RoyalJellyProductionAndProcessing,

        [Display(Name = "Propolis production and processing")]
        PropolisProductionAndProcessing,

        [Display(Name = "Pollen collection and processing")]
        PollenCollectionAndProcessing,

        [Display(Name = "Honey and bee products packaging")]
        HoneyAndBeeProductsPackaging,

        [Display(Name = "Beekeeping equipment manufacturing")]
        BeekeepingEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
