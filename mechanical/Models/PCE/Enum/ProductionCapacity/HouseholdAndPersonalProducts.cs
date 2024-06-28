
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum HouseholdAndPersonalProducts
    {
        [Display(Name = "Cleaning product manufacturing")]
        CleaningProductManufacturing,

        [Display(Name = "Personal care product manufacturing")]
        PersonalCareProductManufacturing,

        [Display(Name = "Laundry detergent manufacturing")]
        LaundryDetergentManufacturing,

        [Display(Name = "Air freshener manufacturing")]
        AirFreshenerManufacturing,

        [Display(Name = "Dishwashing product manufacturing")]
        DishwashingProductManufacturing,

        [Display(Name = "Personal hygiene product manufacturing")]
        PersonalHygieneProductManufacturing,

        [Display(Name = "Insect repellent manufacturing")]
        InsectRepellentManufacturing,

        [Display(Name = "Household and personal product packaging")]
        HouseholdAndPersonalProductPackaging,

        [Display(Name = "Other")]
        Other
    }
}
