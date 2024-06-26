using System;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.File;

namespace mechanical.Models.Dto.ProductionCapacityDto.FileUploadDto
{
    public class FileCreateDto
    {
        // [Display(Name = "File Id")]
        // public Guid Id { get; set; } 

        [Display(Name = "File Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Document Type")]
        public DocumentType Type { get; set; }

        // public Guid? PCEId { get; set; }
        // public required IFormFile File { get; set; }
        public IFormFile File { get; set; }
    }
}
