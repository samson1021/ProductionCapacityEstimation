using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapcityEstimation
{
    public enum MachineryInstalledPlace
    {
        [Display(Name = "Private Owned LHC")]
        PrivateownedLHC,
        [Display(Name = "Industrial Park")]
        Industrialpark,
        
    }
}
