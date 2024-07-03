using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum IndustrialBuildingMachineryType
    {
        [Display(Name = "Industrial (Mfg) Process Line Equipment")]
        IndustrialProcessLineEquipment,

        [Display(Name = "HVAC System, Fuel Station & Security Apparatus e.t.c.")]
        HVACSecurityApparatus,

        [Display(Name = "Hotel, Office, Laboratory Equipment e.t.c.")]
        HotelOfficeLaboratoryEquipment
    }
}
