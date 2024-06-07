
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum PulpAndPaper
    {
        [Display(Name = "Pulp production")]
        PulpProduction,

        [Display(Name = "Paper manufacturing")]
        PaperManufacturing,

        [Display(Name = "Paperboard manufacturing")]
        PaperboardManufacturing,

        [Display(Name = "Specialty paper manufacturing")]
        SpecialtyPaperManufacturing,

        [Display(Name = "Tissue and hygiene product manufacturing")]
        TissueAndHygieneProductManufacturing,

        [Display(Name = "Paper packaging manufacturing")]
        PaperPackagingManufacturing,

        [Display(Name = "Paper recycling")]
        PaperRecycling,

        [Display(Name = "Pulp and paper machinery manufacturing")]
        PulpAndPaperMachineryManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
