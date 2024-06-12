
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum DefenseEquipment
    {
        [Display(Name = "Military vehicle manufacturing")]
        MilitaryVehicleManufacturing,

        [Display(Name = "Aircraft and aerospace systems manufacturing")]
        AircraftAndAerospaceSystemsManufacturing,

        [Display(Name = "Naval vessel manufacturing")]
        NavalVesselManufacturing,

        [Display(Name = "Weapons and ammunition manufacturing")]
        WeaponsAndAmmunitionManufacturing,

        [Display(Name = "Radar and electronic systems manufacturing")]
        RadarAndElectronicSystemsManufacturing,

        [Display(Name = "Communication systems manufacturing")]
        CommunicationSystemsManufacturing,

        [Display(Name = "Protective gear and armor manufacturing")]
        ProtectiveGearAndArmorManufacturing,

        [Display(Name = "Military optics and targeting systems manufacturing")]
        MilitaryOpticsAndTargetingSystemsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
