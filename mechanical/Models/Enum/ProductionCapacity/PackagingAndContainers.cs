
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum PackagingAndContainers
    {
        [Display(Name = "Paper packaging manufacturing")]
        PaperPackagingManufacturing,

        [Display(Name = "Plastic packaging manufacturing")]
        PlasticPackagingManufacturing,

        [Display(Name = "Glass packaging manufacturing")]
        GlassPackagingManufacturing,

        [Display(Name = "Metal packaging manufacturing")]
        MetalPackagingManufacturing,

        [Display(Name = "Flexible packaging manufacturing")]
        FlexiblePackagingManufacturing,

        [Display(Name = "Rigid packaging manufacturing")]
        RigidPackagingManufacturing,

        [Display(Name = "Packaging printing and labeling")]
        PackagingPrintingAndLabeling,

        [Display(Name = "Packaging machinery manufacturing")]
        PackagingMachineryManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
