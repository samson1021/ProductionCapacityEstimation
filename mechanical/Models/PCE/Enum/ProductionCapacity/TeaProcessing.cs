
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum TeaProcessing
    {
        [Display(Name = "Tea cultivation and harvesting")]
        TeaCultivationAndHarvesting,

        [Display(Name = "Tea processing and manufacturing")]
        TeaProcessingAndManufacturing,

        [Display(Name = "Tea blending and packaging")]
        TeaBlendingAndPackaging,

        [Display(Name = "Herbal tea manufacturing")]
        HerbalTeaManufacturing,

        [Display(Name = "Instant tea manufacturing")]
        InstantTeaManufacturing,

        [Display(Name = "Tea flavoring and infusion")]
        TeaFlavoringAndInfusion,

        [Display(Name = "Tea distribution and retail")]
        TeaDistributionAndRetail,

        [Display(Name = "Tea export and import")]
        TeaExportAndImport,

        [Display(Name = "Other")]
        Other
    }
}
