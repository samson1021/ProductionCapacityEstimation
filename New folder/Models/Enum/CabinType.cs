using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum CabinType
    {
        [Display(Name = "Full Cab")]
        FullCab,

        [Display(Name = "Single Cab")]
        SingleCab,

        [Display(Name = "Extra Cab")]
        ExtraCab,

        [Display(Name = "Double Cab")]
        DoubleCab,

        [Display(Name = "Sunny Canopy")]
        SunnyCanopy,

        [Display(Name = "Other")]
        Other,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
