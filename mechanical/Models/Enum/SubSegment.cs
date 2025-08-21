using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum SubSegment
    {
        [Display(Name ="Large Corporate")]
        LargeCorporate,

        [Display(Name = "Medium Corporate")]
        MediumCorporate,

        [Display(Name = "Small Corporate")]
        SmallCorporate,

        [Display(Name = "Wholesale District")]
        WholesaleDistrict,

        [Display(Name = "Cooperative and Financial Institution")]
        CooperativeAndFinancialInstitution,

        [Display(Name = "State Owned Enterprise & Other Public")]
        StateOwnedEnterpriseAndOtherPublic,

        [Display(Name = "Regional Government")]
        RegionalGovernment,

        [Display(Name = "Central Government")]
        CentralGovernment,

        [Display(Name = "Corporate")]
        IFBCorporate,

        [Display(Name = "SME Banking")]
        IFBSMEBanking,

        [Display(Name = "District")]
        IFBDistrict,
        [Display(Name ="Retail")]
        Retail,

        [Display(Name = "Foreclosure")]
        Foreclosure,

        [Display(Name = "Litigation")]
        Litigation,

        [Display(Name = "Acquired Asset Administration")]
        AcquiredAssetAdministration,

        [Display(Name = "Fixed Asset Management")]
        FixedAssetManagement
    }
}
