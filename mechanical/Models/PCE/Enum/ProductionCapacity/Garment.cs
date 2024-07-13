
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum Garment
    {
        [Display(Name = "Clothing manufacturing")]
        ClothingManufacturing,

        [Display(Name = "Apparel accessories manufacturing")]
        ApparelAccessoriesManufacturing,

        [Display(Name = "Textile printing and dyeing")]
        TextilePrintingAndDyeing,

        [Display(Name = "Sewing and stitching services")]
        SewingAndStitchingServices,

        [Display(Name = "Embroidery and embellishment services")]
        EmbroideryAndEmbellishmentServices,

        [Display(Name = "Apparel pattern making and grading")]
        ApparelPatternMakingAndGrading,

        [Display(Name = "Garment washing and finishing")]
        GarmentWashingAndFinishing,

        [Display(Name = "Garment packaging and labeling")]
        GarmentPackagingAndLabeling,

        [Display(Name = "Other")]
        Other
    }
}
