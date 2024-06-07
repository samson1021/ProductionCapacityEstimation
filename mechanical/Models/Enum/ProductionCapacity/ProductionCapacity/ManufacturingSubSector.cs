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
            [Display(Name = " Other")]
            Other
        }
        enum Agriculturalmachinery
        {
            [Display(Name = "Tractor manufacturing")]
            TractorManufacturing,

            [Display(Name = "Combine harvester manufacturing")]
            CombineHarvesterManufacturing,

            [Display(Name = "Irrigation equipment manufacturing")]
            IrrigationEquipmentManufacturing,

            [Display(Name = "Seed processing equipment manufacturing")]
            SeedProcessingEquipmentManufacturing,

            [Display(Name = "Livestock equipment manufacturing")]
            LivestockEquipmentManufacturing,

            [Display(Name = "Farm implement manufacturing")]
            FarmImplementManufacturing,

            [Display(Name = "Agricultural sprayer manufacturing")]
            AgriculturalSprayerManufacturing,

            [Display(Name = "Precision agriculture technology manufacturing")]
            PrecisionAgricultureTechnologyManufacturing,

            [Display(Name = "Other")]
            Other
        }

    }
}
