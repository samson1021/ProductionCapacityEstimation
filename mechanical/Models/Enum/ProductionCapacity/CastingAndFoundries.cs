
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum CastingAndFoundries
    {
        [Display(Name = "Iron casting")]
        IronCasting,

        [Display(Name = "Steel casting")]
        SteelCasting,

        [Display(Name = "Aluminum casting")]
        AluminumCasting,

        [Display(Name = "Copper casting")]
        CopperCasting,

        [Display(Name = "Investment casting")]
        InvestmentCasting,

        [Display(Name = "Sand casting")]
        SandCasting,

        [Display(Name = "Die casting")]
        DieCasting,

        [Display(Name = "Foundry equipment manufacturing")]
        FoundryEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
