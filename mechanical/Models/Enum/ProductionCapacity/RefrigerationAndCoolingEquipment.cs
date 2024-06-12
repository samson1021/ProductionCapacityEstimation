
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum RefrigerationAndCoolingEquipment
    {
        [Display(Name = "Refrigerator manufacturing")]
        RefrigeratorManufacturing,

        [Display(Name = "Air conditioning equipment manufacturing")]
        AirConditioningEquipmentManufacturing,

        [Display(Name = "Commercial refrigeration equipment manufacturing")]
        CommercialRefrigerationEquipmentManufacturing,

        [Display(Name = "Industrial cooling systems manufacturing")]
        IndustrialCoolingSystemsManufacturing,

        [Display(Name = "HVAC systems manufacturing")]
        HVACSystemsManufacturing,

        [Display(Name = "Heat pump manufacturing")]
        HeatPumpManufacturing,

        [Display(Name = "Refrigeration and cooling equipment parts manufacturing")]
        RefrigerationAndCoolingEquipmentPartsManufacturing,

        [Display(Name = "Refrigeration and cooling equipment installation and servicing")]
        RefrigerationAndCoolingEquipmentInstallationAndServicing,

        [Display(Name = "Other")]
        Other
    }
}
