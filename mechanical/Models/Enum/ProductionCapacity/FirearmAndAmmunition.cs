
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum FirearmAndAmmunition
    {
        [Display(Name = "Firearm manufacturing")]
        FirearmManufacturing,

        [Display(Name = "Ammunition manufacturing")]
        AmmunitionManufacturing,

        [Display(Name = "Firearm components manufacturing")]
        FirearmComponentsManufacturing,

        [Display(Name = "Firearm accessories manufacturing")]
        FirearmAccessoriesManufacturing,

        [Display(Name = "Firearm barrel manufacturing")]
        FirearmBarrelManufacturing,

        [Display(Name = "Firearm optics and sights manufacturing")]
        FirearmOpticsAndSightsManufacturing,

        [Display(Name = "Ammunition reloading equipment manufacturing")]
        AmmunitionReloadingEquipmentManufacturing,

        [Display(Name = "Shooting range equipment manufacturing")]
        ShootingRangeEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
