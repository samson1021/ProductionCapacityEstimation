
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum SugarAndConfectionery
    {
        [Display(Name = "Sugar refining")]
        SugarRefining,

        [Display(Name = "Candy and confectionery manufacturing")]
        CandyAndConfectioneryManufacturing,

        [Display(Name = "Chocolate manufacturing")]
        ChocolateManufacturing,

        [Display(Name = "Baking and pastry product manufacturing")]
        BakingAndPastryProductManufacturing,

        [Display(Name = "Sugar and confectionery packaging")]
        SugarAndConfectioneryPackaging,

        [Display(Name = "Sugar substitutes and sweeteners manufacturing")]
        SugarSubstitutesAndSweetenersManufacturing,

        [Display(Name = "Sugar and confectionery distribution and retail")]
        SugarAndConfectioneryDistributionAndRetail,

        [Display(Name = "Sugar and confectionery export and import")]
        SugarAndConfectioneryExportAndImport,

        [Display(Name = "Other")]
        Other
    }
}
