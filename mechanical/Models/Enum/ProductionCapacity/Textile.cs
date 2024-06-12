
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum Textile
    {
        [Display(Name = "Textile fiber production (natural and synthetic)")]
        TextileFiberProduction,

        [Display(Name = "Yarn spinning and twisting")]
        YarnSpinningAndTwisting,

        [Display(Name = "Fabric weaving and knitting")]
        FabricWeavingAndKnitting,

        [Display(Name = "Textile dyeing and printing")]
        TextileDyeingAndPrinting,

        [Display(Name = "Textile finishing and coating")]
        TextileFinishingAndCoating,

        [Display(Name = "Textile manufacturing (apparel, home textiles, etc.)")]
        TextileManufacturing,

        [Display(Name = "Technical textile manufacturing")]
        TechnicalTextileManufacturing,

        [Display(Name = "Textile product distribution and retail")]
        TextileProductDistributionAndRetail,

        [Display(Name = "Other")]
        Other
    }
}
