
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum AnimalFeed
    {
        [Display(Name = "Livestock feed manufacturing")]
        LivestockFeedManufacturing,

        [Display(Name = "Pet food manufacturing")]
        PetFoodManufacturing,

        [Display(Name = "Aqua feed manufacturing")]
        AquaFeedManufacturing,

        [Display(Name = "Poultry feed manufacturing")]
        PoultryFeedManufacturing,

        [Display(Name = "Dairy feed manufacturing")]
        DairyFeedManufacturing,

        [Display(Name = "Feed additives manufacturing")]
        FeedAdditivesManufacturing,

        [Display(Name = "Feed premix manufacturing")]
        FeedPremixManufacturing,

        [Display(Name = "Feed processing equipment manufacturing")]
        FeedProcessingEquipmentManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
