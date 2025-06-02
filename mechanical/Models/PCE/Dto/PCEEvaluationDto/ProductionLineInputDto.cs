using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public abstract class ProductionLineInputBaseDto
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Input Type")]
        public string Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Input Quantity")]
        public decimal Quantity { get; set; }

        [Required]
        [Display(Name = "Input Unit")]
        public MeasurementUnit Unit { get; set; }

        [Display(Name = "Source Type")]
        public SourceType? SourceType { get; set; }
        
        [Display(Name = "Referenced Line")]
        public Guid? ReferencedLineId { get; set; }
    }

    public class ProductionLineInputPostDto : ProductionLineInputBaseDto
    {
    }

    public class ProductionLineInputReturnDto : ProductionLineInputBaseDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductionLineId { get; set; }
    }

    public class ProductionLineInputUpdateDto : ProductionLineInputBaseDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductionLineId { get; set; }
    }
}