
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum SportsAndRecreation
    {
        [Display(Name = "Sports equipment manufacturing")]
        SportsEquipmentManufacturing,

        [Display(Name = "Fitness equipment manufacturing")]
        FitnessEquipmentManufacturing,

        [Display(Name = "Outdoor gear manufacturing")]
        OutdoorGearManufacturing,

        [Display(Name = "Athletic footwear manufacturing")]
        AthleticFootwearManufacturing,

        [Display(Name = "Sports apparel manufacturing")]
        SportsApparelManufacturing,

        [Display(Name = "Sporting goods manufacturing")]
        SportingGoodsManufacturing,

        [Display(Name = "Recreational vehicle manufacturing")]
        RecreationalVehicleManufacturing,

        [Display(Name = "Sports and recreation product distribution")]
        SportsAndRecreationProductDistribution,

        [Display(Name = "Other")]
        Other
    }
}
