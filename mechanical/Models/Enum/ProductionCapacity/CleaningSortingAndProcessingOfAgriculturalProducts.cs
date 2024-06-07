
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum CleaningSortingAndProcessingOfAgriculturalProducts
    {
        [Display(Name = "Grain cleaning and processing")]
        GrainCleaningAndProcessing,

        [Display(Name = "Fruit and vegetable sorting and processing")]
        FruitAndVegetableSortingAndProcessing,

        [Display(Name = "Seed cleaning and processing")]
        SeedCleaningAndProcessing,

        [Display(Name = "Nut processing")]
        NutProcessing,

        [Display(Name = "Coffee cherries hulling")]
        CoffeeCherriesHulling,

        [Display(Name = "Green coffee bean washing")]
        GreenCoffeeBeanWashing,

        [Display(Name = "Green coffee bean sorting and packaging")]
        GreenCoffeeBeanSortingAndPackaging,

        [Display(Name = "Rice milling and processing")]
        RiceMillingAndProcessing,

        [Display(Name = "Other")]
        Other
    }
}
