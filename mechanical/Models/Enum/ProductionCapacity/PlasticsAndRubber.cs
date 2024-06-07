
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum PlasticsAndRubber
    {
        [Display(Name = "Plastic product manufacturing")]
        PlasticProductManufacturing,

        [Display(Name = "Plastic packaging manufacturing")]
        PlasticPackagingManufacturing,

        [Display(Name = "Plastic film and sheet manufacturing")]
        PlasticFilmAndSheetManufacturing,

        [Display(Name = "Plastic pipe and fittings manufacturing")]
        PlasticPipeAndFittingsManufacturing,

        [Display(Name = "Rubber product manufacturing")]
        RubberProductManufacturing,

        [Display(Name = "Rubber tire manufacturing")]
        RubberTireManufacturing,

        [Display(Name = "Synthetic rubber manufacturing")]
        SyntheticRubberManufacturing,

        [Display(Name = "Plastic and rubber recycling")]
        PlasticAndRubberRecycling,

        [Display(Name = "Other")]
        Other
    }
}
