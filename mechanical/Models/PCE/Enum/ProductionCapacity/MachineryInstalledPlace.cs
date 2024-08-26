using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum MachineryInstalledPlace
    {
        [Display(Name = "Private Owned LHC")]
        PrivateownedLHC,
        
        [Display(Name = "Industrial Park")]
        Industrialpark,
        
    }
}
