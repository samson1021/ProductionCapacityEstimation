using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum BodyType
    {
        [Display(Name = "Compact")]
        Compact,

        [Display(Name = "Sedan")]
        Sedan,

        [Display(Name = "Station Wagon")]
        StationWagon,

        [Display(Name = "Pickup")]
        Pickup,

        [Display(Name = "Minibus")]
        Minibus,

        [Display(Name = "Bus")]
        Bus,

        [Display(Name = "Truck, Dry Cargo")]
        TruckDryCargo,

        [Display(Name = "Truck, Liquid Cargo")]
        TruckLiquidCargo,

        [Display(Name = "Others")]
        Others
    }
}
