using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public abstract class ProductionLineBaseDto<TProductionLineInput>
        where TProductionLineInput : ProductionLineInputBaseDto
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Machine/Equipment Name")]
        public string LineName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Output Type")]
        public string OutputType { get; set; }

        [Display(Name = "Output Phase")]
        public OutputPhase? OutputPhase { get; set; }

        [Required]
        [Display(Name = "Is Bottleneck")]
        public bool IsBottleneck { get; set; } = false;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Actual Production Capacity")]
        public decimal ActualCapacity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Design Production Capacity")]
        public decimal? DesignCapacity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Attainable Production Capacity")]
        public decimal? AttainableCapacity { get; set; }

        [Required]
        [Display(Name = "Production Unit")]
        public MeasurementUnit ProductionUnit { get; set; }

        [Required]
        [Display(Name = "Production Measurement")]
        public ProductionMeasurement ProductionMeasurement { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Total Input")]
        public decimal? TotalInput { get; set; }

        [Display(Name = "Conversion Ratio")]
        public string? ConversionRatio { get; set; }
        
        public abstract List<TProductionLineInput> ProductionLineInputs { get; set; }
    }

    public class ProductionLineReturnDto : ProductionLineBaseDto<ProductionLineInputReturnDto>
    {
        public override List<ProductionLineInputReturnDto> ProductionLineInputs { get; set; } = new List<ProductionLineInputReturnDto>();
        public Guid Id { get; set; }
    }

    public class ProductionLinePostDto : ProductionLineBaseDto<ProductionLineInputPostDto>
    {
        public override List<ProductionLineInputPostDto> ProductionLineInputs { get; set; } = new List<ProductionLineInputPostDto>();
    }

    public class ProductionLineUpdateDto : ProductionLineBaseDto<ProductionLineInputUpdateDto>
    {
        public override List<ProductionLineInputUpdateDto> ProductionLineInputs { get; set; } = new List<ProductionLineInputUpdateDto>();
        public Guid? Id { get; set; }
    }
}
