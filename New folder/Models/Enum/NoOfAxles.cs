using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum NoOfAxles
    {
        [Display(Name = "1 Axle")]
        OneAxle,

        [Display(Name = "2 Axles")]
        TwoAxles,

        [Display(Name = "3 Axles")]
        ThreeAxles,

        [Display(Name = "4 Axles")]
        FourAxles,

        [Display(Name = "5 Axles")]
        FiveAxles,

        [Display(Name = "6 Axles")]
        SixAxles,

        [Display(Name = "7 Axles")]
        SevenAxles,

        [Display(Name = "8 Axles")]
        EightAxles,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
