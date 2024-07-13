
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum Electronics
    {
        [Display(Name = "Consumer electronics manufacturing")]
        ConsumerElectronicsManufacturing,

        [Display(Name = "Semiconductor manufacturing")]
        SemiconductorManufacturing,

        [Display(Name = "Printed circuit board (PCB) manufacturing")]
        PrintedCircuitBoardManufacturing,

        [Display(Name = "Electronic component manufacturing")]
        ElectronicComponentManufacturing,

        [Display(Name = "Communication equipment manufacturing")]
        CommunicationEquipmentManufacturing,

        [Display(Name = "Medical electronics manufacturing")]
        MedicalElectronicsManufacturing,

        [Display(Name = "Automotive electronics manufacturing")]
        AutomotiveElectronicsManufacturing,

        [Display(Name = "Electronic assembly and testing")]
        ElectronicAssemblyAndTesting,

        [Display(Name = "Other")]
        Other
    }
}
