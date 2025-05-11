using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;

namespace mechanical.Models.PCE.Dto.PCEEvaluationDto
{
    public abstract class PCEEvaluationBaseDto<TProductionLine, TJustification, TDateTimeRange, TProductionLineInput>
        where TProductionLine : ProductionLineBaseDto<TProductionLineInput>
        where TJustification : JustificationBaseDto
        where TDateTimeRange : DateTimeRangeBaseDto, new()
        where TProductionLineInput : ProductionLineInputBaseDto
    {
        [Required]
        public Guid PCEId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }

        [StringLength(100)]
        [Display(Name = "Country of Origin")]
        public string? CountryOfOrigin { get; set; }

        [Required]
        [Display(Name = "Has Input-Output Data?")]
        public bool HasInputOutputData { get; set; }

        [Display(Name = "Justifications")]
        public List<TJustification> Justifications { get; set; } = new List<TJustification>();

        [Required]
        [Display(Name = "Production Line Type")]
        public ProductionLineType ProductionLineType { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one production line is required")]
        [Display(Name = "Production Line(s)")]
        public List<TProductionLine> ProductionLines { get; set; } = new List<TProductionLine>();

        [Required]
        [StringLength(100)]
        [Display(Name = "Technical Obsolescence Status")]
        public string TechnicalObsolescenceStatus { get; set; }

        [Required]
        [Display(Name = "Machine Functionality Status")]
        public MachineFunctionalityStatus MachineFunctionalityStatus { get; set; }

        [Display(Name = "Non-Functionality Reason")]
        public MachineNonFunctionalityReason? MachineNonFunctionalityReason { get; set; }

        [StringLength(500)]
        [Display(Name = "Other Non-Functionality Reason")]
        public string? OtherMachineNonFunctionalityReason { get; set; }

        [StringLength(1000)]
        [Display(Name = "Factors Affecting Production Capacity")]
        public string? FactorsAffectingProductionCapacity { get; set; }

        [Display(Name = "Time Consumed to Check")]
        public TDateTimeRange TimeConsumedToCheck { get; set; } = new TDateTimeRange();

        [Required]
        [StringLength(200)]
        [Display(Name = "Inspection Place")]
        public string InspectionPlace { get; set; }

        [Required]
        [Display(Name = "Inspection Date")]
        public DateOnly InspectionDate { get; set; }

        [StringLength(2000)]
        [Display(Name = "Survey Remark")]
        public string? SurveyRemark { get; set; }

        [StringLength(2000)]
        [Display(Name = "Remark")]
        public string? Remark { get; set; }
    }

    public class PCEEvaluationPostDto : PCEEvaluationBaseDto<ProductionLinePostDto, JustificationPostDto, DateTimeRangePostDto, ProductionLineInputPostDto>
    {
        [Required]
        // [FileExtensions(Extensions = "jpg,pdf,doc,docx", ErrorMessage = "Only .jpg, .pdf, .doc, .docx files are allowed.")]
        // [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5 MB.")]
        [Display(Name = "Witness Form")]
        public IFormFile WitnessForm { get; set; }

        [Display(Name = "Production Process Flow Diagrams")]
        public List<IFormFile>? ProductionProcessFlowDiagrams { get; set; } = new List<IFormFile>();

        [Display(Name = "Supporting Evidences")]
        public List<IFormFile>? SupportingEvidences { get; set; } = new List<IFormFile>();
    }

    public class PCEEvaluationReturnDto : PCEEvaluationBaseDto<ProductionLineReturnDto, JustificationReturnDto, DateTimeRangeReturnDto, ProductionLineInputReturnDto>
    {
        public Guid Id { get; set; }
        public ProductionCapacity PCE { get; set; }

        [Display(Name = "Evaluated By")]
        public Guid EvaluatorId { get; set; }

        [Display(Name = "Bottleneck Production Line")]
        public BottleneckProductionLineDto? BottleneckProductionLine { get; set; }

        [Display(Name = "Total Capacity")]
        public decimal TotalCapacity { get; set; }

        [Display(Name = "Witness Form")]
        public ReturnFileDto WitnessForm { get; set; }

        [Display(Name = "Production Process Flow Diagrams")]
        public List<ReturnFileDto> ProductionProcessFlowDiagrams { get; set; } = new List<ReturnFileDto>();

        [Display(Name = "Supporting Evidences")]
        public List<ReturnFileDto> SupportingEvidences { get; set; } = new List<ReturnFileDto>();

        [Display(Name = "Uploaded Files")]
        public List<ReturnFileDto> UploadedFiles { get; set; } = new List<ReturnFileDto>();

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }

        [Display(Name = "Created By")]
        public Guid? CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated By")]
        public Guid? UpdatedBy { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class PCEEvaluationUpdateDto : PCEEvaluationBaseDto<ProductionLineUpdateDto, JustificationUpdateDto, DateTimeRangeUpdateDto, ProductionLineInputUpdateDto>
    {
        [Required]
        public Guid Id { get; set; }
        public ProductionCapacity PCE { get; set; }

        [Display(Name = "Evaluated By")]
        public Guid EvaluatorId { get; set; }

        [Display(Name = "Witness Form")]
        public ReturnFileDto WitnessForm { get; set; }

        [Display(Name = "Production Process Flow Diagrams")]
        public List<ReturnFileDto>? ProductionProcessFlowDiagrams { get; set; } = new List<ReturnFileDto>();

        [Display(Name = "Supporting Evidences")]
        public List<ReturnFileDto>? SupportingEvidences { get; set; } = new List<ReturnFileDto>();

        public IFormFile? NewWitnessForm { get; set; }
        public List<IFormFile>? NewProductionProcessFlowDiagrams { get; set; }
        public List<IFormFile>? NewSupportingEvidences { get; set; }
        public List<Guid>? DeletedFileIds { get; set; }
    }

    public abstract class JustificationBaseDto
    {
        [Required]
        public JustificationReason Reason { get; set; }

        [StringLength(1000)]
        public string? JustificationText { get; set; }
    }

    public class JustificationPostDto: JustificationBaseDto
    {

    }

    public class JustificationUpdateDto: JustificationBaseDto
    {
        public Guid? Id { get; set; }
    }
    public class JustificationReturnDto: JustificationBaseDto
    {
        public Guid Id { get; set; }
    }

    public class BottleneckProductionLineDto
    {
        [Required]
        [StringLength(200)]
        public string LineName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Capacity { get; set; }

        [Required]
        public MeasurementUnit Unit { get; set; }
    }

    public abstract class DateTimeRangeBaseDto
    {
        [Required]
        [Display(Name = "Start DateTime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "End DateTime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
    }
    
    public class DateTimeRangePostDto: DateTimeRangeBaseDto
    {

    }

    public class DateTimeRangeUpdateDto: DateTimeRangeBaseDto
    {

    }

    public class DateTimeRangeReturnDto: DateTimeRangeBaseDto
    {
        [Display(Name = "Duration")]
        public TimeSpan Duration => End - Start;
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file && file.Length > _maxFileSize)
            {
                return new ValidationResult(ErrorMessage ?? $"File size cannot exceed {_maxFileSize / (1024 * 1024)} MB.");
            }

            return ValidationResult.Success;
        }
    }
}
