
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum SteelFabrication
    {
        [Display(Name = "Structural steel fabrication")]
        StructuralSteelFabrication,

        [Display(Name = "Steel plate fabrication")]
        SteelPlateFabrication,

        [Display(Name = "Steel beam and column fabrication")]
        SteelBeamAndColumnFabrication,

        [Display(Name = "Steel bridge and infrastructure fabrication")]
        SteelBridgeAndInfrastructureFabrication,

        [Display(Name = "Steel pipe and tube fabrication")]
        SteelPipeAndTubeFabrication,

        [Display(Name = "Steel staircase and railing fabrication")]
        SteelStaircaseAndRailingFabrication,

        [Display(Name = "Steel tank and vessel fabrication")]
        SteelTankAndVesselFabrication,

        [Display(Name = "Steel fabrication welding and assembly")]
        SteelFabricationWeldingAndAssembly,

        [Display(Name = "Other")]
        Other
    }
}
