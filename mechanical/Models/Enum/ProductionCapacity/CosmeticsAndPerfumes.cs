
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum CosmeticsAndPerfumes
    {
        [Display(Name = "Skincare product manufacturing")]
        SkincareProductManufacturing,

        [Display(Name = "Makeup product manufacturing")]
        MakeupProductManufacturing,

        [Display(Name = "Haircare product manufacturing")]
        HaircareProductManufacturing,

        [Display(Name = "Fragrance manufacturing")]
        FragranceManufacturing,

        [Display(Name = "Personal care product manufacturing")]
        PersonalCareProductManufacturing,

        [Display(Name = "Cosmetics packaging manufacturing")]
        CosmeticsPackagingManufacturing,

        [Display(Name = "Cosmetic tools and accessories manufacturing")]
        CosmeticToolsAndAccessoriesManufacturing,

        [Display(Name = "Cosmeceutical manufacturing")]
        CosmeceuticalManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
