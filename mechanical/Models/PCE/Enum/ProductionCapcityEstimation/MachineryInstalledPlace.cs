using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacityEstimation
{
    public enum MachineryInstalledPlace
    {
        [Display(Name = "Private Owned LHC")]
        PrivateownedLHC,
        [Display(Name = "Industrial Park")]
        Industrialpark,
        
    }
}
