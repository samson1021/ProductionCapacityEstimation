
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum MetalFinishingAndCoating
    {
        [Display(Name = "Surface coating and painting services")]
        SurfaceCoatingAndPaintingServices,

        [Display(Name = "Electroplating and metal plating services")]
        ElectroplatingAndMetalPlatingServices,

        [Display(Name = "Powder coating services")]
        PowderCoatingServices,

        [Display(Name = "Anodizing services")]
        AnodizingServices,

        [Display(Name = "Galvanizing services")]
        GalvanizingServices,

        [Display(Name = "Metal polishing and buffing services")]
        MetalPolishingAndBuffingServices,

        [Display(Name = "Metal passivation services")]
        MetalPassivationServices,

        [Display(Name = "Metal surface treatment chemicals manufacturing")]
        MetalSurfaceTreatmentChemicalsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
