
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum Marine
    {
        [Display(Name = "Shipbuilding")]
        Shipbuilding,

        [Display(Name = "Boat manufacturing")]
        BoatManufacturing,

        [Display(Name = "Marine engine manufacturing")]
        MarineEngineManufacturing,

        [Display(Name = "Marine electronics manufacturing")]
        MarineElectronicsManufacturing,

        [Display(Name = "Marine propulsion systems manufacturing")]
        MarinePropulsionSystemsManufacturing,

        [Display(Name = "Marine safety equipment manufacturing")]
        MarineSafetyEquipmentManufacturing,

        [Display(Name = "Marine navigation and communication equipment manufacturing")]
        MarineNavigationAndCommunicationEquipmentManufacturing,

        [Display(Name = "Marine vessel repair and maintenance services")]
        MarineVesselRepairAndMaintenanceServices,

        [Display(Name = "Other")]
        Other
    }
}
