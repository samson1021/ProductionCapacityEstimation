
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum SoapAndDetergents
    {
        [Display(Name = "Soap manufacturing")]
        SoapManufacturing,

        [Display(Name = "Detergent manufacturing")]
        DetergentManufacturing,

        [Display(Name = "Laundry soap manufacturing")]
        LaundrySoapManufacturing,

        [Display(Name = "Liquid soap manufacturing")]
        LiquidSoapManufacturing,

        [Display(Name = "Dishwashing detergent manufacturing")]
        DishwashingDetergentManufacturing,

        [Display(Name = "Industrial soap and detergent manufacturing")]
        IndustrialSoapAndDetergentManufacturing,

        [Display(Name = "Soap and detergent packaging")]
        SoapAndDetergentPackaging,

        [Display(Name = "Soap and detergent distribution and retail")]
        SoapAndDetergentDistributionAndRetail,

        [Display(Name = "Other")]
        Other
    }
}
