
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum ICTEquipment
    {
        [Display(Name = "Computer hardware manufacturing")]
        ComputerHardwareManufacturing,

        [Display(Name = "Networking equipment manufacturing")]
        NetworkingEquipmentManufacturing,

        [Display(Name = "Telecommunications equipment manufacturing")]
        TelecommunicationsEquipmentManufacturing,

        [Display(Name = "Data storage device manufacturing")]
        DataStorageDeviceManufacturing,

        [Display(Name = "Server and data center equipment manufacturing")]
        ServerAndDataCenterEquipmentManufacturing,

        [Display(Name = "Mobile device manufacturing")]
        MobileDeviceManufacturing,

        [Display(Name = "ICT peripherals manufacturing")]
        ICTPeripheralsManufacturing,

        [Display(Name = "ICT equipment assembly and testing")]
        ICTEquipmentAssemblyAndTesting,

        [Display(Name = "Other")]
        Other
    }
}
