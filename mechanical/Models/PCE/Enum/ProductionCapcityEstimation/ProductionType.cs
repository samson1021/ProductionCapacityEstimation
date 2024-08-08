using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacityEstimation
{
    public enum ProductionType
    {
        [Display(Name = "Manufacturing")]
        Manufacturing,
        [Display(Name = "Plant")]
        Plant,
    }
}
