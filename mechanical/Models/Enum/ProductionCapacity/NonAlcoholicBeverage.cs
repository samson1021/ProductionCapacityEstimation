
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum NonAlcoholicBeverage
    {
        [Display(Name = "Bottled water manufacturing")]
        BottledWaterManufacturing,

        [Display(Name = "Soft drink manufacturing")]
        SoftDrinkManufacturing,

        [Display(Name = "Juice and beverage concentrate manufacturing")]
        JuiceAndBeverageConcentrateManufacturing,

        [Display(Name = "Energy drink manufacturing")]
        EnergyDrinkManufacturing,

        [Display(Name = "Ready-to-drink coffee and tea manufacturing")]
        ReadyToDrinkCoffeeAndTeaManufacturing,

        [Display(Name = "Sports and vitamin drink manufacturing")]
        SportsAndVitaminDrinkManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
