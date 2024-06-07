
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum PreciousMetals
    {
        [Display(Name = "Gold mining and refining")]
        GoldMiningAndRefining,

        [Display(Name = "Silver mining and refining")]
        SilverMiningAndRefining,

        [Display(Name = "Platinum group metals mining and refining")]
        PlatinumGroupMetalsMiningAndRefining,

        [Display(Name = "Precious metal jewelry manufacturing")]
        PreciousMetalJewelryManufacturing,

        [Display(Name = "Precious metal bullion manufacturing")]
        PreciousMetalBullionManufacturing,

        [Display(Name = "Precious metal coin manufacturing")]
        PreciousMetalCoinManufacturing,

        [Display(Name = "Precious metal recycling and refining")]
        PreciousMetalRecyclingAndRefining,

        [Display(Name = "Precious metal investment and trading")]
        PreciousMetalInvestmentAndTrading,

        [Display(Name = "Other")]
        Other
    }
}
