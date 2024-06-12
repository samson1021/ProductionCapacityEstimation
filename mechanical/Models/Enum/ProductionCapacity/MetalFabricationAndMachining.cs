
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum MetalFabricationAndMachining
    {
        [Display(Name = "Sheet metal fabrication")]
        SheetMetalFabrication,

        [Display(Name = "Welding and metal joining services")]
        WeldingAndMetalJoiningServices,

        [Display(Name = "CNC machining services")]
        CNCMachiningServices,

        [Display(Name = "Metal stamping and forming")]
        MetalStampingAndForming,

        [Display(Name = "Metal casting")]
        MetalCasting,

        [Display(Name = "Metal cutting and sawing services")]
        MetalCuttingAndSawingServices,

        [Display(Name = "Metal surface treatment and finishing")]
        MetalSurfaceTreatmentAndFinishing,

        [Display(Name = "Metal fabrication tool and equipment manufacturing")]
        MetalFabricationToolAndEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
