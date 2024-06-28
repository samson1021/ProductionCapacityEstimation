using System;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Enum.PCEEnums.File;

namespace mechanical.Models.PCE.Dto.FileUploadDto
{

    public class FileReturnDto
    {
        [Display(Name = "File Id")]
        public Guid Id { get; set; } 

        [Display(Name = "File Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Document Type")]
        public DocumentType Type { get; set; }

        // public Guid? PCEId { get; set; }

        public string ContentType { get; set; } = string.Empty;
    }

}
