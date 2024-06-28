
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum BuildingMaterials
    {
        [Display(Name = "Cement manufacturing")]
        CementManufacturing,

        [Display(Name = "Brick and tile manufacturing")]
        BrickAndTileManufacturing,

        [Display(Name = "Glass manufacturing")]
        GlassManufacturing,

        [Display(Name = "Roofing materials manufacturing")]
        RoofingMaterialsManufacturing,

        [Display(Name = "Insulation materials manufacturing")]
        InsulationMaterialsManufacturing,

        [Display(Name = "Flooring materials manufacturing")]
        FlooringMaterialsManufacturing,

        [Display(Name = "Wood products manufacturing")]
        WoodProductsManufacturing,

        [Display(Name = "Steel and metal products manufacturing")]
        SteelAndMetalProductsManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
