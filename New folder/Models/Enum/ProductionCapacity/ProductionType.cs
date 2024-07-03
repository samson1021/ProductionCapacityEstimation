using System.ComponentModel.DataAnnotations;
namespace mechanical.Models.Enum
{
    public enum ProductionType
    {
        [Display(Name = "Manufacturing")]
        Manufacturing,
        [Display(Name = "Plant")]
        Plant,
    }
}
