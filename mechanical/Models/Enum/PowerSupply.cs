using System.ComponentModel.DataAnnotations;
namespace mechanical.Models.Enum
{
    public enum PowerSupply
    {
        [Display(Name = "220/1P/50Hz")]
        Power220SinglePhase50Hz,

        [Display(Name = "380/3P/50Hz")]
        Power380ThreePhase50Hz,

        [Display(Name = "Other")]
        Other,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
