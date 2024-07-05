using System;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Enum.File;

namespace mechanical.Models.PCE.Dto.FileUploadDto
{
    public class FileCreateDto
    {

        [Display(Name = "Document Type")]
        public Category Category { get; set; }
        public required IFormFile File { get; set; }

        public Guid? PCEId { get; set; }
    }
}
