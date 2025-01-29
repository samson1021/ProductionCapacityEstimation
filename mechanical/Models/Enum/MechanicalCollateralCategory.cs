using System.ComponentModel.DataAnnotations;
namespace mechanical.Models.Enum
{
    public enum MechanicalCollateralCategory
    {
        [Display(Name = "MOTOR VEHICLE")]
        MOV,
        [Display(Name = "CONST, MNG & AGR MACHINERY")]
        CMAMachinery,
        [Display(Name = "IND (Mfg) & BLDG FACILITY EQUIPMENT")]
        IBFEqupment
    }
}
