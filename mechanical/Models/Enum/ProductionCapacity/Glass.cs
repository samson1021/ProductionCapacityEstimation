
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum Glass
    {
        [Display(Name = "Flat glass manufacturing")]
        FlatGlassManufacturing,

        [Display(Name = "Glass container manufacturing")]
        GlassContainerManufacturing,

        [Display(Name = "Fiberglass manufacturing")]
        FiberglassManufacturing,

        [Display(Name = "Glassware manufacturing")]
        GlasswareManufacturing,

        [Display(Name = "Automotive glass manufacturing")]
        AutomotiveGlassManufacturing,

        [Display(Name = "Glass fiber manufacturing")]
        GlassFiberManufacturing,

        [Display(Name = "Glass cutting and processing")]
        GlassCuttingAndProcessing,

        [Display(Name = "Glass coating and laminating")]
        GlassCoatingAndLaminating,

        [Display(Name = "Other")]
        Other
    }
}
