
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum PrintingAndWriting
    {
        [Display(Name = "Commercial printing services")]
        CommercialPrintingServices,

        [Display(Name = "Offset printing")]
        OffsetPrinting,

        [Display(Name = "Digital printing")]
        DigitalPrinting,

        [Display(Name = "Screen printing")]
        ScreenPrinting,
        [Display(Name = "Flexographic printing")]
        FlexographicPrinting,

        [Display(Name = "Gravure printing")]
        GravurePrinting,

        [Display(Name = "Printing ink manufacturing")]
        PrintingInkManufacturing,

        [Display(Name = "Paper and stationery manufacturing")]
        PaperAndStationeryManufacturing,

        [Display(Name = "Other")]
        Other
    }
}
