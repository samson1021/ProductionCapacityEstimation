
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum Energy
    {
        [Display(Name = "Renewable energy equipment manufacturing (solar, wind, hydro)")]
        RenewableEnergyEquipmentManufacturing,

        [Display(Name = "Power generation equipment manufacturing")]
        PowerGenerationEquipmentManufacturing,

        [Display(Name = "Energy storage system manufacturing")]
        EnergyStorageSystemManufacturing,

        [Display(Name = "Transmission and distribution equipment manufacturing")]
        TransmissionAndDistributionEquipmentManufacturing,

        [Display(Name = "Energy management systems manufacturing")]
        EnergyManagementSystemsManufacturing,

        [Display(Name = "Energy metering and monitoring equipment manufacturing")]
        EnergyMeteringAndMonitoringEquipmentManufacturing,

        [Display(Name = "Energy efficiency solutions manufacturing")]
        EnergyEfficiencySolutionsManufacturing,

        [Display(Name = "Power plant construction and equipment manufacturing")]
        PowerPlantConstructionAndEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
