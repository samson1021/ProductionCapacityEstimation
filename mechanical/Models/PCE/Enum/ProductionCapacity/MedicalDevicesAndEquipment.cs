
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum MedicalDevicesAndEquipment
    {
        [Display(Name = "Surgical instrument manufacturing")]
        SurgicalInstrumentManufacturing,

        [Display(Name = "Imaging and diagnostic equipment manufacturing")]
        ImagingAndDiagnosticEquipmentManufacturing,

        [Display(Name = "Patient monitoring equipment manufacturing")]
        PatientMonitoringEquipmentManufacturing,

        [Display(Name = "Medical laboratory equipment manufacturing")]
        MedicalLaboratoryEquipmentManufacturing,

        [Display(Name = "Medical consumables manufacturing")]
        MedicalConsumablesManufacturing,

        [Display(Name = "Dental equipment manufacturing")]
        DentalEquipmentManufacturing,

        [Display(Name = "Rehabilitation and mobility aids manufacturing")]
        RehabilitationAndMobilityAidsManufacturing,

        [Display(Name = "Medical device packaging and sterilization")]
        MedicalDevicePackagingAndSterilization,

        [Display(Name = "Other")]
        Other
    }
}
