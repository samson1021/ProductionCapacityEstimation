
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum IndustrialEquipment
    {
        [Display(Name = "Heavy machinery manufacturing")]
        HeavyMachineryManufacturing,

        [Display(Name = "Industrial automation equipment manufacturing")]
        IndustrialAutomationEquipmentManufacturing,

        [Display(Name = "Material handling equipment manufacturing")]
        MaterialHandlingEquipmentManufacturing,

        [Display(Name = "Industrial pumps and compressors manufacturing")]
        IndustrialPumpsAndCompressorsManufacturing,

        [Display(Name = "Industrial filtration and separation equipment manufacturing")]
        IndustrialFiltrationAndSeparationEquipmentManufacturing,

        [Display(Name = "Industrial heating and cooling equipment manufacturing")]
        IndustrialHeatingAndCoolingEquipmentManufacturing,

        [Display(Name = "Industrial mixing and blending equipment manufacturing")]
        IndustrialMixingAndBlendingEquipmentManufacturing,

        [Display(Name = "Industrial equipment maintenance and repair services")]
        IndustrialEquipmentMaintenanceAndRepairServices,

        [Display(Name = "Other")]
        Other
    }
}
