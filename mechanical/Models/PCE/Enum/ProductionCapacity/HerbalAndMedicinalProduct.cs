
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum HerbalAndMedicinalProduct
    {
        [Display(Name = "Herbal medicine manufacturing")]
        HerbalMedicineManufacturing,

        [Display(Name = "Traditional medicine manufacturing")]
        TraditionalMedicineManufacturing,

        [Display(Name = "Herbal supplements manufacturing")]
        HerbalSupplementsManufacturing,

        [Display(Name = "Herbal skincare products manufacturing")]
        HerbalSkincareProductsManufacturing,

        [Display(Name = "Herbal extracts and tinctures manufacturing")]
        HerbalExtractsAndTincturesManufacturing,

        [Display(Name = "Medicinal tea manufacturing")]
        MedicinalTeaManufacturing,

        [Display(Name = "Herbal product packaging")]
        HerbalProductPackaging,

        [Display(Name = "Herbal product research and development")]
        HerbalProductResearchAndDevelopment,

        [Display(Name = "Other")]
        Other
    }
}
