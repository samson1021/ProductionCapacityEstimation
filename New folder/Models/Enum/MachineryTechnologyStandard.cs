using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum MachineryTechnologyStandard
    {
        [Display(Name = "Western Europe")]
        WesternEurope,

        [Display(Name = "Eastern Europe")]
        EasternEurope,

        [Display(Name = "North America")]
        NorthAmerica,

        [Display(Name = "South America")]
        SouthAmerica,

        [Display(Name = "Asia")]
        Asia,

        [Display(Name = "Australia")]
        Australia,

        [Display(Name = "Antarctica")]
        Antarctica,

        [Display(Name = "Africa")]
        Africa,

        [Display(Name = "Japan Standard")]
        JapanStandard,

        [Display(Name = "China Product which is European & American Standard")]
        ChinaEuropeanAmericanStandard,

        [Display(Name = "Brazil for Coffee Processing Machinery")]
        BrazilCoffeeProcessing
    }
}
