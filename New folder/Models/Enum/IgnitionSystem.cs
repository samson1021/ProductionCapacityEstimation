using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum IgnitionSystem
    {
        [Display(Name = "Spark Ignition")]
        SparkIgnition,

        [Display(Name = "Compression Ignition")]
        CompressionIgnition,

        [Display(Name = "N/A")]
        NotApplicable
    }
}
