
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum ChemicalsAndFertilizers
    {
        [Display(Name = "Petrochemical manufacturing")]
        PetrochemicalManufacturing,

        [Display(Name = "Agrochemical manufacturing")]
        AgrochemicalManufacturing,

        [Display(Name = "Specialty chemical manufacturing")]
        SpecialtyChemicalManufacturing,

        [Display(Name = "Organic chemical manufacturing")]
        OrganicChemicalManufacturing,

        [Display(Name = "Inorganic chemical manufacturing")]
        InorganicChemicalManufacturing,

        [Display(Name = "Fertilizer manufacturing")]
        FertilizerManufacturing,

        [Display(Name = "Adhesive and sealant manufacturing")]
        AdhesiveAndSealantManufacturing,

        [Display(Name = "Paint and coating manufacturing")]
        PaintAndCoatingManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
