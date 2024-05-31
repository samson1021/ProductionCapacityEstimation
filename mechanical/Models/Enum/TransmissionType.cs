using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum TransmissionType
    {
        [Display(Name = "Manual")]
        Manual,

        [Display(Name = "Automatic")]
        Automatic,

        [Display(Name = "Semi-Automatic")]
        SemiAutomatic,

        [Display(Name = "N/A")]
        NotApplicable,

        [Display(Name = "Other")]
        Other
    }
}
