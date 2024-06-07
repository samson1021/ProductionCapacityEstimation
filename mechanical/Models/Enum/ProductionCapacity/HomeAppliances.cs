
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum HomeAppliances
    {
        [Display(Name = "Kitchen appliances manufacturing")]
        KitchenAppliancesManufacturing,

        [Display(Name = "Laundry appliances manufacturing")]
        LaundryAppliancesManufacturing,

        [Display(Name = "Refrigeration appliances manufacturing")]
        RefrigerationAppliancesManufacturing,

        [Display(Name = "Heating and cooling appliances manufacturing")]
        HeatingAndCoolingAppliancesManufacturing,

        [Display(Name = "Cleaning appliances manufacturing")]
        CleaningAppliancesManufacturing,

        [Display(Name = "Small home appliances manufacturing")]
        SmallHomeAppliancesManufacturing,

        [Display(Name = "Smart home appliances manufacturing")]
        SmartHomeAppliancesManufacturing,

        [Display(Name = "Home appliance parts manufacturing")]
        HomeAppliancePartsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
