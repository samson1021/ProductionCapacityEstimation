using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCEId))]
    public class PCEEvaluation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PCEId { get; set; }

        [Required]
        public Guid EvaluatorId { get; set; }

        [Required]
        [StringLength(200)]
        public string MachineName { get; set; }

        [StringLength(100)]
        public string? CountryOfOrigin { get; set; }

        [Required]
        public bool HasInputOutputData { get; set; }
        
        public virtual ICollection<Justification> Justifications { get; set; } = new List<Justification>();

        [Required]
        public ProductionLineType ProductionLineType { get; set; }
        public virtual ICollection<ProductionLine> ProductionLines { get; set; } = new List<ProductionLine>();

        [Required]
        [StringLength(100)]
        public string TechnicalObsolescenceStatus { get; set; }
        // public ObsolescenceStatus TechnicalObsolescenceStatus { get; set; }

        [Required]
        public MachineFunctionalityStatus MachineFunctionalityStatus { get; set; }
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }

        [StringLength(500)]
        public string? OtherMachineNonFunctionalityReason { get; set; }

        [StringLength(1000)]
        public string? FactorsAffectingProductionCapacity { get; set; }

        public virtual DateTimeRange? TimeConsumedToCheck { get; set; }

        [Required]
        [StringLength(200)]
        public string InspectionPlace { get; set; }

        public DateOnly InspectionDate { get; set; }

        [StringLength(2000)]
        public string? SurveyRemark { get; set; }

        [StringLength(2000)]
        public string? Remark { get; set; }

        public DateTime? CompletedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("PCEId")]
        public virtual ProductionCapacity PCE { get; set; }
        [ForeignKey("EvaluatorId")]
        public virtual User Evaluator { get; set; }
    }

    [Index(nameof(PCEEvaluationId))]
    public class Justification 
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PCEEvaluationId { get; set; }

        [Required]
        public JustificationReason Reason { get; set; }

        [StringLength(1000)]
        public string? JustificationText { get; set; }

        [ForeignKey("PCEEvaluationId")]
        public virtual PCEEvaluation PCEEvaluation { get; set; }
    }
    
    [Index(nameof(PCEEvaluationId))]
    public class DateTimeRange
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PCEEvaluationId { get; set; }

        [ForeignKey("PCEEvaluationId")]
        public virtual PCEEvaluation PCEEvaluation { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public bool Contains(DateTime dateTime) => dateTime >= Start && dateTime <= End;
        
        [NotMapped]
        public TimeSpan Duration => End - Start;

        [NotMapped]
        public bool IsValid => End > Start;

        public void Validate()
        {
            if (End <= Start)
            {
                throw new ValidationException("End must be after Start.");
            }
        }
    }
}