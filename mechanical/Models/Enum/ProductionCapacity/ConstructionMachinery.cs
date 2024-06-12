
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum ConstructionMachinery
    {
        [Display(Name = "Excavator manufacturing")]
        ExcavatorManufacturing,

        [Display(Name = "Bulldozer manufacturing")]
        BulldozerManufacturing,

        [Display(Name = "Crane manufacturing")]
        CraneManufacturing,

        [Display(Name = "Paver manufacturing")]
        PaverManufacturing,

        [Display(Name = "Concrete pump manufacturing")]
        ConcretePumpManufacturing,

        [Display(Name = "Backhoe loader manufacturing")]
        BackhoeLoaderManufacturing,

        [Display(Name = "Construction equipment attachments manufacturing")]
        ConstructionEquipmentAttachmentsManufacturing,

        [Display(Name = "Road roller manufacturing")]
        RoadRollerManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
