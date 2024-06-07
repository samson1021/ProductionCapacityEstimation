
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum Aerospace
    {
        [Display(Name = "Aircraft manufacturing")]
        AircraftManufacturing,

        [Display(Name = "Spacecraft manufacturing")]
        SpacecraftManufacturing,

        [Display(Name = "Satellite manufacturing")]
        SatelliteManufacturing,

        [Display(Name = "Aircraft engine manufacturing")]
        AircraftEngineManufacturing,

        [Display(Name = "Avionics manufacturing")]
        AvionicsManufacturing,

        [Display(Name = "Missile and defense system manufacturing")]
        MissileAndDefenseSystemManufacturing,

        [Display(Name = "Drone manufacturing")]
        DroneManufacturing,

        [Display(Name = "Aerospace composite materials manufacturing")]
        AerospaceCompositeMaterialsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}