
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum MiningAndQuarrying
    {
        [Display(Name = "Coal mining")]
        CoalMining,

        [Display(Name = "Metal ore mining")]
        MetalOreMining,

        [Display(Name = "Stone quarrying")]
        StoneQuarrying,

        [Display(Name = "Mineral extraction and processing")]
        MineralExtractionAndProcessing,

        [Display(Name = "Sand and gravel mining")]
        SandAndGravelMining,

        [Display(Name = "Salt mining")]
        SaltMining,

        [Display(Name = "Phosphate mining")]
        PhosphateMining,

        [Display(Name = "Mining equipment manufacturing")]
        MiningEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
