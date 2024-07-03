using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum EquipmentCondition
    {
        [Display(Name = "New")]
        New,

        [Display(Name = "Functional & Take Overhaul Maintenance")]
        FunctionalWithOverhaul,

        [Display(Name = "Functional but Does Not Take Overhaul Maintenance")]
        FunctionalWithoutOverhaul,

        [Display(Name = "Not Functional & Requires Minor Repair")]
        NotFunctionalMinorRepair,

        [Display(Name = "Not Functional & Requires Medium Repair")]
        NotFunctionalMediumRepair,

        [Display(Name = "Not Operational due to Major Components and Parts Missing")]
        NotOperationalMajorComponentsMissing,

        [Display(Name = "Not Operational due to Minor Replaceable Parts Missing")]
        NotOperationalMinorPartsMissing
    }
}
