
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum BakeryAndSnackFood
    {
        [Display(Name = "Bread and pastry manufacturing")]
        BreadAndPastryManufacturing,

        [Display(Name = "Snack food manufacturing")]
        SnackFoodManufacturing,

        [Display(Name = "Tortilla and chip manufacturing")]
        TortillaAndChipManufacturing,

        [Display(Name = "Confectionery manufacturing")]
        ConfectioneryManufacturing,

        [Display(Name = "Biscuit manufacturing")]
        BiscuitManufacturing,

        [Display(Name = "Bakery and snack food packaging")]
        BakeryAndSnackFoodPackaging,

        [Display(Name = "Bakery equipment manufacturing")]
        BakeryEquipmentManufacturing,

        [Display(Name = "Snack food seasoning and flavoring manufacturing")]
        SnackFoodSeasoningAndFlavoringManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
