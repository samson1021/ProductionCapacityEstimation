using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
     //namespace ManufacturingSubSector
    public class ManufacturingSubSector
    {
        enum Aerospace
        {
            [Display(Name = "Aircraft manufacturing")]
            Aircraftmanufacturing,
            [Display(Name = "Spacecraft manufacturing")]
            Spacecraftmanufacturing,
            [Display(Name = "Satellite manufacturing")]
            Satellitemanufacturing,
            [Display(Name = "Aircraft engine manufacturing")]
            Aircraftenginemanufacturing,
            [Display(Name = "Avionics manufacturingg")]
            Avionicsmanufacturing,
            [Display(Name = "Missile and defense system manufacturing")]
            Missileanddefensesystemmanufacturing,
            [Display(Name = "Drone manufacturing")]
            Dronemanufacturing,
            [Display(Name = "Aerospace composite materials manufacturing")]
            Aerospacecompositematerialsmanufacturing,
            [Display(Name = " Others, please specify")]
            Otherspleasespecify
        }
        enum Agriculturalmachinery
        {
            A,
            B,
            c
        }

    }
}
