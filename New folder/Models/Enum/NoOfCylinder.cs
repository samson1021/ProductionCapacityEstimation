using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum NoOfCylinder
    {
        [Display(Name = "2 Cylinders Inline")]
        TwoCylindersInline,

        [Display(Name = "3 Cylinders Inline")]
        CylindersInline,

        [Display(Name = "4 Cylinders Inline")]
        FourCylindersInline,

        [Display(Name = "6 Cylinders Inline")]
        SixCylindersInline,

        [Display(Name = "V6 Cylinders Inline")]
        V6CylindersInline,

        [Display(Name = "8 Cylinders Inline")]
        EightCylindersInline,

        [Display(Name = "V8 Cylinders Inline")]
        V8CylindersInline,

        [Display(Name = "Other")]
        Other,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
