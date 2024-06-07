
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum PetroleumAndCoalProducts
    {
        [Display(Name = "Refining of crude oil")]
        RefiningOfCrudeOil,

        [Display(Name = "Petroleum product manufacturing (gasoline, diesel, jet fuel, etc.)")]
        PetroleumProductManufacturing,

        [Display(Name = "Petrochemical manufacturing")]
        PetrochemicalManufacturing,

        [Display(Name = "Asphalt manufacturing")]
        AsphaltManufacturing,

        [Display(Name = "Lubricant manufacturing")]
        LubricantManufacturing,

        [Display(Name = "Coal processing and manufacturing")]
        CoalProcessingAndManufacturing,

        [Display(Name = "Charcoal manufacturing")]
        CharcoalManufacturing,

        [Display(Name = "Petroleum and coal product storage and distribution")]
        PetroleumAndCoalProductStorageAndDistribution,

        [Display(Name = "Other")]
        Other
    }
}
