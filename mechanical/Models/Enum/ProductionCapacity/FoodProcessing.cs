
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.ProductionCapacity
{
    public enum FoodProcessing
    {
        [Display(Name = "Fruit and vegetable processing")]
        FruitAndVegetableProcessing,

        [Display(Name = "Meat processing")]
        MeatProcessing,

        [Display(Name = "Dairy product processing")]
        DairyProductProcessing,

        [Display(Name = "Grain milling and processing")]
        GrainMillingAndProcessing,

        [Display(Name = "Snack food processing")]
        SnackFoodProcessing,

        [Display(Name = "Frozen food processing")]
        FrozenFoodProcessing,

        [Display(Name = "Canned food processing")]
        CannedFoodProcessing,

        [Display(Name = "Food packaging and labeling")]
        FoodPackagingAndLabeling,

        [Display(Name = "Other")]
        Other
    }
}
