
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum AlcoholicBeverageSubsector
    {
        [Display(Name = "Brewery")]
        Brewery,

        [Display(Name = "Winery")]
        Winery,

        [Display(Name = "Distillery")]
        Distillery,

        [Display(Name = "Cider production")]
        CiderProduction,

        [Display(Name = "Craft beer production")]
        CraftBeerProduction,

        [Display(Name = "Spirit production")]
        SpiritProduction,

        [Display(Name = "Wine bottling and packaging")]
        WineBottlingAndPackaging,

        [Display(Name = "Alcoholic beverage labeling")]
        AlcoholicBeverageLabeling,

        [Display(Name = "Other")]
        Other
    }
}
