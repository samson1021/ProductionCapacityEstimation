using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum ConstructionMiningAgriculturalMachineryType
    {
        [Display(Name = "Compacting, Concrete Pump & Crusher Machinery")]
        CompactingConcretePumpCrusher,

        [Display(Name = "Agriculture, Lifting & Drilling rig Machinery")]
        AgricultureLiftingDrilling,

        [Display(Name = "Earthmoving Machinery")]
        Earthmoving,

        [Display(Name = "Road Surface & Fire Fighting Machinery")]
        RoadSurfaceFireFighting
    }
}
