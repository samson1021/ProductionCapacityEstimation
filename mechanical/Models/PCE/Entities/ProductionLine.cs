using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCEEvaluationId))]
    public class ProductionLine
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("PCEEvaluation")]
        public Guid PCEEvaluationId { get; set; }

        [Required]
        public Guid EvaluatorId { get; set; }

        [Required]
        [StringLength(200)]
        public string LineName { get; set; }

        [Required]
        [StringLength(100)]
        public string OutputType { get; set; }

        public OutputPhase? OutputPhase { get; set; }

        [Required]
        public bool IsBottleneck { get; set; }

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

        public virtual PCEEvaluation? PCEEvaluation { get; set; }
        public virtual CreateUser? Evaluator { get; set; }
    }

    public class ProductionLineInput
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("ProductionLine")]
        public Guid ProductionLineId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        public MeasurementUnit Unit { get; set; }

        public virtual ProductionLine? ProductionLine { get; set; }
    }
}