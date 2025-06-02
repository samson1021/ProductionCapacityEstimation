using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCEEvaluationId))]
    public class ProductionLine
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PCEEvaluationId { get; set; }

        [Required]
        [StringLength(200)]
        public string LineName { get; set; }

        [Required]
        [StringLength(100)]
        public string OutputType { get; set; }

        public OutputPhase? OutputPhase { get; set; }

        [Required]
        public bool IsBottleneck { get; set; } = false;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ActualCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DesignCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AttainableCapacity { get; set; }

        [Required]
        public MeasurementUnit ProductionUnit { get; set; }
        
        [Required]
        public ProductionMeasurement ProductionMeasurement { get; set; }

        public virtual ICollection<ProductionLineInput> ProductionLineInputs { get; set; } = new List<ProductionLineInput>();

        [Range(0, double.MaxValue)]
        public decimal? TotalInput { get; set; }

        public decimal? ConversionRatio { get; set; }

        [ForeignKey("PCEEvaluationId")]
        public virtual PCEEvaluation PCEEvaluation { get; set; }
    }

    [Index(nameof(ProductionLineId))]
    public class ProductionLineInput
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductionLineId { get; set; }
        public Guid? ReferencedLineId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        public MeasurementUnit Unit { get; set; }

        public SourceType? SourceType { get; set; }

        [ForeignKey("ProductionLineId")]
        public virtual ProductionLine ProductionLine { get; set; }

        [ForeignKey("ReferencedLineId")]
        public virtual ProductionLine? ReferencedLine { get; set; }
    }
}