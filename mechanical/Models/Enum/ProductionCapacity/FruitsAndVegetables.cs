
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enums.ProductionCapacity
{
    public enum FruitsAndVegetables
    {
        [Display(Name = "Fresh Produce Sorting and Grading")]
        FreshProduceSortingAndGrading,

        [Display(Name = "Fruit and Vegetable Processing")]
        FruitAndVegetableProcessing,

        [Display(Name = "Fruit and Vegetable Juices and Concentrates")]
        FruitAndVegetableJuicesAndConcentrates,

        [Display(Name = "Frozen Fruits and Vegetables")]
        FrozenFruitsAndVegetables,

        [Display(Name = "Fruit and Vegetable Snacks")]
        FruitAndVegetableSnacks,

        [Display(Name = "Fruit and Vegetable Sauces and Condiments")]
        FruitAndVegetableSaucesAndCondiments,

        [Display(Name = "Fruit and Vegetable Extracts and Powders")]
        FruitAndVegetableExtractsAndPowders,

        [Display(Name = "Fruit and Vegetable Packaging")]
        FruitAndVegetablePackaging,

        [Display(Name = "Other")]
        Other
    }
}
