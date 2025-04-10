using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum Segment
    {
        [Display(Name ="Wholesale Conventional")]
        WholesaleConventional,

        [Display(Name = "Wholesale IFB")]
        WholesaleIFB,

        [Display(Name = "Retail")]
        Retail,

        [Display(Name = "Foreclosure")]
        Foreclosure,

        [Display(Name = "Litigation")]
        Litigation,

        [Display(Name = "Acquired Asset Administration")]
        AcquiredAssetAdministration,

        [Display(Name = "Facility Management")]
        FacilityManagement
    }
}
