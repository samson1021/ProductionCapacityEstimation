
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum Pharmaceuticals
    {
        [Display(Name = "Pharmaceutical research and development")]
        PharmaceuticalResearchAndDevelopment,

        [Display(Name = "Pharmaceutical manufacturing")]
        PharmaceuticalManufacturing,

        [Display(Name = "Generic drug manufacturing")]
        GenericDrugManufacturing,

        [Display(Name = "Biopharmaceutical manufacturing")]
        BiopharmaceuticalManufacturing,

        [Display(Name = "Pharmaceutical packaging and labeling")]
        PharmaceuticalPackagingAndLabeling,

        [Display(Name = "Pharmaceutical distribution and wholesaling")]
        PharmaceuticalDistributionAndWholesaling,

        [Display(Name = "Pharmaceutical testing and analysis")]
        PharmaceuticalTestingAndAnalysis,

        [Display(Name = "Pharmaceutical contract manufacturing")]
        PharmaceuticalContractManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
