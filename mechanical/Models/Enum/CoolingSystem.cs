using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum CoolingSystem
    {
        [Display(Name = "Air Cooled")]
        AirCooled,

        [Display(Name = "Water Cooled")]
        WaterCooled,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
