
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum ElectricalEquipment
    {
        [Display(Name = "Transformer manufacturing")]
        TransformerManufacturing,

        [Display(Name = "Switchgear and control panel manufacturing")]
        SwitchgearAndControlPanelManufacturing,

        [Display(Name = "Electrical motor manufacturing")]
        ElectricalMotorManufacturing,

        [Display(Name = "Power distribution equipment manufacturing")]
        PowerDistributionEquipmentManufacturing,

        [Display(Name = "Wiring device and electrical connector manufacturing")]
        WiringDeviceAndElectricalConnectorManufacturing,

        [Display(Name = "Electrical cable and wire manufacturing")]
        ElectricalCableAndWireManufacturing,

        [Display(Name = "Circuit breaker and fuse manufacturing")]
        CircuitBreakerAndFuseManufacturing,

        [Display(Name = "Electrical component manufacturing")]
        ElectricalComponentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
