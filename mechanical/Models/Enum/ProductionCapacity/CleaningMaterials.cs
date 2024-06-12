
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum CleaningMaterials
    {
        [Display(Name = "Household cleaning product manufacturing")]
        HouseholdCleaningProductManufacturing,

        [Display(Name = "Industrial cleaning product manufacturing")]
        IndustrialCleaningProductManufacturing,

        [Display(Name = "Laundry detergent manufacturing")]
        LaundryDetergentManufacturing,

        [Display(Name = "Cleaning tools and equipment manufacturing")]
        CleaningToolsAndEquipmentManufacturing,

        [Display(Name = "Cleaning chemical manufacturing")]
        CleaningChemicalManufacturing,

        [Display(Name = "Air fresheners and deodorizers manufacturing")]
        AirFreshenersAndDeodorizersManufacturing,

        [Display(Name = "Personal hygiene product manufacturing")]
        PersonalHygieneProductManufacturing,

        [Display(Name = "Cleaning product packaging and labeling")]
        CleaningProductPackagingAndLabeling,

        [Display(Name = "Other")]
        Other
    }
}
