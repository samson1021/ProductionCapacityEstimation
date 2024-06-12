
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum MetalWork
    {
        [Display(Name = "Metal forging and pressing")]
        MetalForgingAndPressing,

        [Display(Name = "Metal machining and turning")]
        MetalMachiningAndTurning,

        [Display(Name = "Metal welding and fabrication")]
        MetalWeldingAndFabrication,

        [Display(Name = "Metal bending and forming")]
        MetalBendingAndForming,

        [Display(Name = "Metal cutting and shearing")]
        MetalCuttingAndShearing,

        [Display(Name = "Metal drilling and tapping")]
        MetalDrillingAndTapping,

        [Display(Name = "Metal punching and stamping")]
        MetalPunchingAndStamping,

        [Display(Name = "Metal assembly and sub-assembly")]
        MetalAssemblyAndSubAssembly,

        [Display(Name = "Other")]
        Other
    }
}
