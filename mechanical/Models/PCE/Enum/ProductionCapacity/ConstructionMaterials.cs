
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum ConstructionMaterials
    {
        [Display(Name = "Concrete product manufacturing")]
        ConcreteProductManufacturing,

        [Display(Name = "Bricks and blocks manufacturing")]
        BricksAndBlocksManufacturing,

        [Display(Name = "Roofing materials manufacturing")]
        RoofingMaterialsManufacturing,

        [Display(Name = "Insulation materials manufacturing")]
        InsulationMaterialsManufacturing,

        [Display(Name = "Doors and windows manufacturing")]
        DoorsAndWindowsManufacturing,

        [Display(Name = "Plumbing fixtures manufacturing")]
        PlumbingFixturesManufacturing,

        [Display(Name = "Flooring materials manufacturing")]
        FlooringMaterialsManufacturing,

        [Display(Name = "Structural steel manufacturing")]
        StructuralSteelManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
