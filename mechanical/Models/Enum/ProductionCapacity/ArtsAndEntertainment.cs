
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum ArtsAndEntertainment
    {
        [Display(Name = "Film production")]
        FilmProduction,

        [Display(Name = "Music instrument manufacturing")]
        MusicInstrumentManufacturing,

        [Display(Name = "Toy manufacturing")]
        ToyManufacturing,

        [Display(Name = "Costume and prop manufacturing")]
        CostumeAndPropManufacturing,

        [Display(Name = "Stage equipment manufacturing")]
        StageEquipmentManufacturing,

        [Display(Name = "Recording studio equipment manufacturing")]
        RecordingStudioEquipmentManufacturing,

        [Display(Name = "Event production equipment manufacturing")]
        EventProductionEquipmentManufacturing,

        [Display(Name = "Art and entertainment software development")]
        ArtAndEntertainmentSoftwareDevelopment,

        [Display(Name = "Other")]
        Other
    }
}
