
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum MotorVehicle
    {
        [Display(Name = "Passenger car manufacturing")]
        PassengerCarManufacturing,

        [Display(Name = "Commercial vehicle manufacturing")]
        CommercialVehicleManufacturing,

        [Display(Name = "Electric vehicle manufacturing")]
        ElectricVehicleManufacturing,

        [Display(Name = "Automotive component manufacturing")]
        AutomotiveComponentManufacturing,

        [Display(Name = "Automotive assembly and testing")]
        AutomotiveAssemblyAndTesting,

        [Display(Name = "Automotive chassis and body manufacturing")]
        AutomotiveChassisAndBodyManufacturing,

        [Display(Name = "Automotive engine manufacturing")]
        AutomotiveEngineManufacturing,

        [Display(Name = "Automotive parts and accessories manufacturing")]
        AutomotivePartsAndAccessoriesManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
