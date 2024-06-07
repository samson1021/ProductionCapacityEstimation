
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum Sponge
    {
        [Display(Name = "Mattress manufacturing")]
        MattressManufacturing,

        [Display(Name = "Pillow manufacturing")]
        PillowManufacturing,

        [Display(Name = "Cleaning product manufacturing (sponges, scrubbers, etc.)")]
        CleaningProductManufacturing,

        [Display(Name = "Automotive cleaning product manufacturing")]
        AutomotiveCleaningProductManufacturing,

        [Display(Name = "Medical device manufacturing (foam padding, wound dressings, etc.)")]
        MedicalDeviceManufacturing,

        [Display(Name = "Soundproofing and insulation product manufacturing")]
        SoundproofingAndInsulationProductManufacturing,

        [Display(Name = "Sponge and foam product packaging")]
        SpongeAndFoamProductPackaging,

        [Display(Name = "Sponge and foam product customization and fabrication")]
        SpongeAndFoamProductCustomizationAndFabrication,

        [Display(Name = "Other")]
        Other
    }
}
