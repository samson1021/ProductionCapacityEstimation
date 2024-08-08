using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacityEstimation
{
    public enum IndustrialPark
    {
        [Display(Name = "Adama")]
        Adama,
        [Display(Name = "BahirDar")]
        BahirDar,
        [Display(Name = "Bole lemi I")]
        BolelemiI,
        [Display(Name = "Bole lemi II")]
        BolelemiII,
        [Display(Name = "Kilinto")]
        Kilinto,
        [Display(Name = "Combolcha")]
        Combolcha,
        [Display(Name = "Debrebirhan")]
        Debrebirhan,
        [Display(Name = "Dukem Eastern Industry zone")]
        DukemEasternIndustryzone,
        [Display(Name = "Hawassa")]
        Hawassa,
        [Display(Name = "Jimma")]
        Jimma,
        [Display(Name = "Mekelle")]
        Mekelle,        
        [Display(Name = "Others please specify")]
        Otherspleasespecify
    }
}
