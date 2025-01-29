
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum LeatherAndLeatherProducts
    {
        [Display(Name = "Leather goods manufacturing (bags, belts, wallets)")]
        LeatherGoodsManufacturing,

        [Display(Name = "Footwear manufacturing")]
        FootwearManufacturing,

        [Display(Name = "Leather garment manufacturing")]
        LeatherGarmentManufacturing,

        [Display(Name = "Leather upholstery manufacturing")]
        LeatherUpholsteryManufacturing,

        [Display(Name = "Leather accessories manufacturing")]
        LeatherAccessoriesManufacturing,

        [Display(Name = "Leather tanning and processing")]
        LeatherTanningAndProcessing,

        [Display(Name = "Leather care and maintenance products manufacturing")]
        LeatherCareAndMaintenanceProductsManufacturing,

        [Display(Name = "Leather and leather products export and distribution")]
        LeatherAndLeatherProductsExportAndDistribution,

        [Display(Name = "Other")]
        Other
    }
}
