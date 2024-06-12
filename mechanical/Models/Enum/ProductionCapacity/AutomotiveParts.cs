
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum AutomotiveParts
    {
        [Display(Name = "Engine parts manufacturing")]
        EnginePartsManufacturing,

        [Display(Name = "Brake system manufacturing")]
        BrakeSystemManufacturing,

        [Display(Name = "Electrical components manufacturing")]
        ElectricalComponentsManufacturing,

        [Display(Name = "Transmission and drivetrain manufacturing")]
        TransmissionAndDrivetrainManufacturing,

        [Display(Name = "Exhaust system manufacturing")]
        ExhaustSystemManufacturing,

        [Display(Name = "Automotive electronics manufacturing")]
        AutomotiveElectronicsManufacturing,

        [Display(Name = "Automotive glass manufacturing")]
        AutomotiveGlassManufacturing,

        [Display(Name = "Automotive seating and interiors manufacturing")]
        AutomotiveSeatingAndInteriorsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
